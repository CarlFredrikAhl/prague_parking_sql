using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace prague_parking_sql
{
    public class Vehicle
    {
        //What you can chooose
        public enum VehicleEnum { Car, Mc }

        public readonly string regNumber;

        public DateTime timeOfArrival = DateTime.Now;

        //The objects type
        VehicleEnum thisVehicleType;

        //Get, set
        public VehicleEnum VehicleType
        {
            get { return this.thisVehicleType; }
            set { this.thisVehicleType = value; }
        }

        public Vehicle(VehicleEnum thisVehicleType, string regNumber)
        {
            this.thisVehicleType = thisVehicleType;
            this.regNumber = regNumber;
        }

        public Vehicle()
        {

        }
    }
}
