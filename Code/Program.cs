using System;
using CsvHelper;
using CsvHelper.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using System.Globalization;
using System.Data.SqlClient;
using CsvHelper.TypeConversion;
using System.Net;
using System.Net.Http;
using System.Data;

namespace GasShipmentScript
{
    class Program
    {

        static string connectionString;

        static async Task Main(string[] args)
        {
            Console.WriteLine("Please enter a SQL connection string:");

            connectionString = Console.ReadLine();
            bool validCon = CheckConnection();

            while (!validCon)
            {
                Console.WriteLine("Invalid connection string\n");
                Console.WriteLine("Please enter a SQL connection string:");

                connectionString = Console.ReadLine();
                validCon = CheckConnection();
            }

            Console.WriteLine("Fetching data...\n");

            await GetDataFromAllCycles(DateTime.Today);
            await GetDataFromAllCycles(DateTime.Today.AddDays(-1));
            await GetDataFromAllCycles(DateTime.Today.AddDays(-2));

            Console.WriteLine("Data stored succesfully.");
        }

        private static bool CheckConnection()
        {
            if (string.IsNullOrWhiteSpace(connectionString))
                return false;

            SqlConnection con;

            try
            {
                con = new SqlConnection(connectionString);
                Console.WriteLine("Verifying SQL connection...\n");
                con.Open();
            }
            catch (ArgumentException)
            {
                return false;
            }
            catch (SqlException)
            {
                return false;
            }

            if (con.State == ConnectionState.Open)
                con.Close();

            Console.WriteLine("Connection valid.\n");

            return true;
        }

        private static async Task GetDataFromAllCycles(DateTime date)
        {
            for (int cycle = 0; cycle < 6; cycle++)
            {
                await GetShipmentData(date, cycle);
            }
        }

        private static async Task GetShipmentData(DateTime date, int cycle)
        {
            string dateString = Uri.EscapeDataString(date.ToString("MM/dd/yyyy"));
            string fields = $"?f=csv&extension=csv&asset=TW&gasDay={dateString}&cycle={cycle}&searchType=NOM&searchString=&locType=ALL&locZone=ALL";

            using (HttpClient client = new HttpClient())
            {
                Uri endpoint = new Uri("https://twtransfer.energytransfer.com/ipost/TW/capacity/operationally-available" + fields);
                var req = await client.GetAsync(endpoint).ContinueWith(async res =>
                {
                    var result = res.Result;
                    if (result.StatusCode == HttpStatusCode.OK)
                    {
                        var filestream = await result.Content.ReadAsStreamAsync();

                        using (StreamReader sr = new StreamReader(filestream))
                        {
                            using (CsvReader csvr = new CsvReader(sr, CultureInfo.InvariantCulture))
                            {
                                csvr.Context.RegisterClassMap<RecordClassMap>();
                                try
                                {
                                    var records = csvr.GetRecords<Record>().ToList();

                                    using (SqlConnection con = new SqlConnection(connectionString))
                                    {
                                        string cycleName = GetCycleName(cycle);
                                        string queary = @"INSERT INTO GasShipments (Loc, LocZn, LocName, LocPurpDesc, LocQTI, FlowInd, DC, OPC, TSQ, OAC, IT, AuthOverrunInd, NomCapExceedInd, AllQtyAvail, QtyReason, Date, Cycle)"; SqlCommand command;

                                        con.Open();
                                        foreach (Record record in records)
                                        {
                                            try
                                            {
                                                string values = $" VALUES ({record.Loc}, '{record.LocZn}', '{record.LocName}', '{record.LocPurpDesc}', '{record.LocQTI}', '{record.FlowInd}', {record.DC}, {record.OPC}, {record.TSQ}, {record.OAC}, {(record.IT ? 1 : 0)}, {(record.AuthOverrunInd ? 1 : 0)}, {(record.NomCapExceedInd ? 1 : 0)}, {(record.AllQtyAvail ? 1 : 0)}, '{record.QtyReason}', '{date.ToString("yyyy-MM-dd")}', '{cycleName}')";
                                                command = new SqlCommand(queary + values, con);
                                                command.CommandType = System.Data.CommandType.Text;
                                                command.ExecuteNonQuery();

                                                Console.WriteLine($"Record (Loc: {record.Loc}, FlowInd: {record.FlowInd}, Date: {date}, Cycle: {cycleName}) added.");
                                            }
                                            catch (SqlException)
                                            {
                                                Console.Error.WriteLine($"Record (Loc: {record.Loc}, FlowInd: {record.FlowInd}, Date: {date}, Cycle: {cycleName}) already exists.");
                                            }
                                        }
                                        con.Close();
                                    }
                                }
                                catch (HeaderValidationException e)
                                {
                                    Console.Error.WriteLine(e.Message);
                                }
                                catch (FieldValidationException e)
                                {
                                    Console.Error.WriteLine(e.Message);
                                }
                                catch (TypeConverterException e)
                                {
                                    Console.Error.WriteLine(e.Message);
                                }
                            }
                        }
                    }
                });
            }
        }

        private static string GetCycleName(int cycle)
        {
            switch (cycle)
            {
                case 0:
                    return "Timely";
                case 1:
                    return "Evening";
                case 2:
                    return "Intraday 1";
                case 3:
                    return "Intraday 2";
                case 4:
                    return "Intraday 3";
                case 5:
                    return "Final";
            }

            return null;
        }
    }

    public class RecordClassMap : ClassMap<Record>
    {
        public RecordClassMap()
        {
            Map(m => m.Loc).Name("Loc");
            Map(m => m.LocZn).Name("Loc Zn")
                .Validate(s => !string.IsNullOrWhiteSpace(s.Field) && s.Field.Length <= 30);
            Map(m => m.LocName).Name("Loc Name")
                .Validate(s => !string.IsNullOrWhiteSpace(s.Field) && s.Field.Length <= 50);
            Map(m => m.LocPurpDesc).Name("Loc Purp Desc")
                .Validate(s => s.Field == "MQ" || s.Field == "M2");
            Map(m => m.LocQTI).Name("Loc/QTI")
                .Validate(s => s.Field == "RPQ" || s.Field == "DPQ");
            Map(m => m.FlowInd).Name("Flow Ind")
                .Validate(s => s.Field == "R" || s.Field == "D");
            Map(m => m.DC).Name("DC");
            Map(m => m.OPC).Name("OPC");
            Map(m => m.TSQ).Name("TSQ");
            Map(m => m.OAC).Name("OAC");
            Map(m => m.IT).Name("IT")
                .TypeConverterOption.BooleanValues(true, true, "Y")
                .TypeConverterOption.BooleanValues(false, true, "N");
            Map(m => m.AuthOverrunInd).Name("Auth Overrun Ind")
                .TypeConverterOption.BooleanValues(true, true, "Y")
                .TypeConverterOption.BooleanValues(false, true, "N");
            Map(m => m.NomCapExceedInd).Name("Nom Cap Exceed Ind")
                .TypeConverterOption.BooleanValues(true, true, "Y")
                .TypeConverterOption.BooleanValues(false, true, "N");
            Map(m => m.AllQtyAvail).Name("All Qty Avail")
                .TypeConverterOption.BooleanValues(true, true, "Y")
                .TypeConverterOption.BooleanValues(false, true, "N");
            Map(m => m.QtyReason).Name("Qty Reason");
        }
    }

    public class Record
    {
        public int Loc { get; set; }
        public string LocZn { get; set; }
        public string LocName { get; set; }
        public string LocPurpDesc { get; set; }
        public string LocQTI { get; set; }
        public string FlowInd { get; set; }
        public int DC { get; set; }
        public int OPC { get; set; }
        public int TSQ { get; set; }
        public int OAC { get; set; }
        public bool IT { get; set; }
        public bool AuthOverrunInd { get; set; }
        public bool NomCapExceedInd { get; set; }
        public bool AllQtyAvail { get; set; }
        public string QtyReason { get; set; }
    }
}