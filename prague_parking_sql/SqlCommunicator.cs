using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace prague_parking_sql
{
    public class SqlCommunicator
    {
        static SqlConnection conn = null;
        static SqlCommand cmd = new SqlCommand();
        static SqlDataReader reader = null;

        public string Save(string regNumber, int vehicleTypeID)
        {
            StringBuilder sb = new StringBuilder();

            conn = new SqlConnection("Data source=(local)\\SqlExpress; database=PragueParking; Integrated Security=true;");

            string query = "exec AddVehicle @regNr, @vehicleTypeID";

            cmd.CommandText = query;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@vehicleTypeID", vehicleTypeID);
            cmd.Parameters.AddWithValue("@regNr", regNumber);
            cmd.Connection = conn;

            try
            {
                conn.Open();
                    
                cmd.ExecuteNonQuery();

                conn.Close();
                conn.Dispose();

                conn = new SqlConnection("Data source=(local)\\SqlExpress; database=PragueParking; Integrated Security=true;");
                conn.Open();
                
                query = "select top 1 * from HiredLots order by LotID desc";
                cmd.CommandText = query;
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@regNr", regNumber);
                cmd.Connection = conn;

                reader = cmd.ExecuteReader();

                sb.Append("Vehicle parked on parking lot ");

                while(reader.Read())
                {
                    string lotNr = reader["LotID"].ToString();
                    sb.Append(lotNr);
                }

                conn.Close();
                conn.Dispose();

            } catch(SqlException se)
            {
                sb.Clear();
                sb.Append(se.Message);
                return sb.ToString();
            }

            return sb.ToString();
        }

        public string GetVehicle(string regNumber)
        {
            StringBuilder sb = new StringBuilder();

            conn = new SqlConnection("Data source=(local)\\SqlExpress; database=PragueParking; Integrated Security=true;");

            string query = "select RegNr, LotID, VehicleTypeID, TimeOfArrival from HiredLots where RegNr = @regNr";

            cmd.CommandText = query;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@regNr", regNumber);
            cmd.Connection = conn;

            try
            {
                conn.Open();

                reader = cmd.ExecuteReader();

                string regNr = string.Empty;
                string lotNr = string.Empty;
                string timeOfArrival = string.Empty;
                string vehicleTypeID = string.Empty;

                while (reader.Read())
                {
                    regNr = reader["RegNr"].ToString();

                    lotNr = reader["LotID"].ToString();

                    vehicleTypeID = reader["vehicleTypeID"].ToString();

                    timeOfArrival = reader["TimeOfArrival"].ToString();
                }

                if (!string.IsNullOrEmpty(vehicleTypeID) && int.Parse(vehicleTypeID) == 1)
                {
                    sb.Append("\nYour car with registration number " + regNr + " is parked on parking lot " +
                        lotNr + " and has been parked there since " + timeOfArrival + "\n");
                
                } else if(!string.IsNullOrEmpty(vehicleTypeID) && int.Parse(vehicleTypeID) == 2)
                {
                    sb.Append("\nYour mc with registration number " + regNr + " is parked on parking lot " +
                        lotNr + " and has been parked there since " + timeOfArrival + "\n");
                
                } else
                {
                    sb.Append("\nYour vehicle is not parked\n");
                }

                conn.Close();
                conn.Dispose();

                conn = new SqlConnection("Data source=(local)\\SqlExpress; database=PragueParking; Integrated Security=true;");
                conn.Open();

                query = "exec GetVehicle @regNr";
                cmd.CommandText = query;
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@regNr", regNumber);
                cmd.Connection = conn;

                cmd.ExecuteNonQuery();

                conn.Close();
                conn.Dispose();

                conn = new SqlConnection("Data source=(local)\\SqlExpress; database=PragueParking; Integrated Security=true;");
                conn.Open();

                query = "select TotalPrice from HistoryHiredLots where RegNr = @regNr";
                cmd.CommandText = query;
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@regNr", regNumber);
                cmd.Connection = conn;

                reader = cmd.ExecuteReader();

                string totalPrice = string.Empty;

                while(reader.Read())
                {
                    totalPrice = reader["TotalPrice"].ToString();
                }

                sb.Append("You will pay no less than " + totalPrice + " swedish kr." + "\n");
            }
            catch (SqlException se)
            {

            }

            return sb.ToString();
        }

        public string MoveVehicle(string regNr, int moveToLotNr)
        {
            StringBuilder sb = new StringBuilder();

            conn = new SqlConnection("Data source=(local)\\SqlExpress; database=PragueParking; Integrated Security=true;");

            string query = "exec MoveVehicle @regNr, @moveToLotNr";

            cmd.CommandText = query;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@moveToLotNr", moveToLotNr);
            cmd.Parameters.AddWithValue("@regNr", regNr);
            cmd.Connection = conn;

            try
            {
                conn.Open();

                cmd.ExecuteNonQuery();

                conn.Close();
                conn.Dispose();

            } catch(SqlException se)
            {
                sb.Append("\n" + se.Message + "\n");
                return sb.ToString();
            }

            sb.Append("\n");
            sb.Append("The vehicle was succefully moved to parking lot " + moveToLotNr + "\n");
            return sb.ToString();
        }

        public string CurrentPlots()
        {
            StringBuilder sb = new StringBuilder();

            conn = new SqlConnection("Data source=(local)\\SqlExpress; database=PragueParking; Integrated Security=true;");
            cmd.CommandText = "select l.LotNr, hl.RegNr, v.VehicleName, hl.TimeOfArrival from HiredLots hl join Lots l on hl.LotID = l.LotID join VehicleType v on hl.VehicleTypeID = v.VehicleID";
            cmd.Connection = conn;

            using (conn)
            {
                try
                {
                    conn.Open();
                    reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        int lot = int.Parse(reader["LotNr"].ToString());
                        string regNumber = reader["RegNr"].ToString();
                        string vehicleName = reader["VehicleName"].ToString();
                        string timeOfArrival = reader["TimeOfArrival"].ToString();

                        sb.Append("Lot number: " + lot + "\n");
                        sb.Append("Registration number: " + regNumber + "\n");
                        sb.Append("Vehicle name: " + vehicleName + "\n");
                        sb.Append("Time of arrival: " + timeOfArrival + "\n");

                        sb.Append("\n");
                    }
                }
                catch (SqlException se)
                {
                    sb.Append(se.Message);
                    return sb.ToString();

                }
                catch (Exception e)
                {

                }
            }

            if(!string.IsNullOrEmpty(sb.ToString()))
            {
                return sb.ToString();
            
            } else
            {
                return "There are currently no vehicles parked \n";
            }
        }

        public string SearchVehicle(string regNr)
        {
            StringBuilder sb = new StringBuilder();

            conn = new SqlConnection("Data source=(local)\\SqlExpress; database=PragueParking; Integrated Security=true;");
            cmd.CommandText = "select l.LotNr, hl.RegNr, v.VehicleName, hl.TimeOfArrival from HiredLots hl join Lots l on hl.LotID = l.LotID join VehicleType v on hl.VehicleTypeID = v.VehicleID where hl.RegNr = @regNr";
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@regNr", regNr);
            cmd.Connection = conn;

            using (conn)
            {
                try
                {
                    conn.Open();
                    reader = cmd.ExecuteReader();

                    int lot = 0;
                    string regNumber = string.Empty;
                    string vehicleName = string.Empty;
                    string timeOfArrival = string.Empty;

                    while (reader.Read())
                    {
                        lot = int.Parse(reader["LotNr"].ToString());
                        regNumber = reader["RegNr"].ToString();
                        vehicleName = reader["VehicleName"].ToString();
                        timeOfArrival = reader["TimeOfArrival"].ToString();
                    }

                    if(string.IsNullOrEmpty(vehicleName))
                    {
                        sb.Append("\n");
                        sb.Append("Your vehicle is not parked");
                        sb.Append("\n");
                        return sb.ToString();
                    }

                    sb.Append("\n");
                    sb.Append("Your " + vehicleName + " with registration number " + regNr + " is parked on parking lot " + lot 
                        + " and has been since " + timeOfArrival);
                    sb.Append("\n");
                }
                catch (SqlException se)
                {
                    sb.Append(se.Message);
                    return sb.ToString();
                }
                catch (Exception e)
                {
                    sb.Append(e.Message);
                    return sb.ToString();
                }

                return sb.ToString();
            }
        }

        public string HistoryPlots()
        {
            StringBuilder sb = new StringBuilder();

            conn = new SqlConnection("Data source=(local)\\SqlExpress; database=PragueParking; Integrated Security=true;");
            cmd.CommandText = "select l.LotNr, hhl.RegNr, v.VehicleName, hhl.TimeOfArrival, hhl.TimeOfCheckout, hhl.TotalPrice from HistoryHiredLots hhl join Lots l on hhl.LotID = l.LotID join VehicleType v on hhl.VehicleTypeID = v.VehicleID order by l.LotNr asc";
            cmd.Connection = conn;

            using (conn)
            {
                try
                {
                    conn.Open();
                    reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        int lot = int.Parse(reader["LotNr"].ToString());
                        string regNumber = reader["RegNr"].ToString();
                        string vehicleName = reader["VehicleName"].ToString();
                        string timeOfArrival = reader["TimeOfArrival"].ToString();
                        string timeOfCheckout = reader["TimeOfCheckout"].ToString();
                        string totalPrice = reader["TotalPrice"].ToString();

                        sb.Append("Lot number: " + lot + "\n");
                        sb.Append("Registration number: " + regNumber + "\n");
                        sb.Append("Vehicle name: " + vehicleName + "\n");
                        sb.Append("Time of arrival: " + timeOfArrival + "\n");
                        sb.Append("Time of checkout: " + timeOfCheckout + "\n");
                        sb.Append("Total price: " + totalPrice + "\n");

                        sb.Append("\n");
                    }
                }
                catch (SqlException se)
                {
                    sb.Append(se.Message);
                    return sb.ToString();

                }
                catch (Exception e)
                {

                }
            }

            if (!string.IsNullOrEmpty(sb.ToString()))
            {
                return sb.ToString();

            }
            else
            {
                return "There are no history \n";
            }
        }
    }
}
