using System;
using System.Collections.Generic;
using System.Text;

namespace MTC.JMIS.AISTrackPublisher
{
    public static class ShipTypes
    {
        public static string GetShipTypes(int ShipId) {
            try
            {
                string shipType = "";
                switch (ShipId)
                {
                    case 0:
                        shipType = "Not available (default)";
                        break;
                    case 1:
                    case 2:
                    case 3:
                    case 4:
                    case 5:
                    case 6:
                    case 7:
                    case 8:
                    case 9:
                    case 10:
                    case 11:
                    case 12:
                    case 13:
                    case 14:
                    case 15:
                    case 16:
                    case 17:
                    case 18:
                    case 19:
                        shipType = "Reserved for future use";
                        break;
                    case 20:
                        shipType = "Wing in ground (WIG), all ships of this type";
                        break;
                    case 21:
                        shipType = "Wing in ground (WIG), Hazardous category A";
                        break;
                    case 22:
                        shipType = "Wing in ground (WIG), Hazardous category B";
                        break;
                    case 23:
                        shipType = "Wing in ground (WIG), Hazardous category C";
                        break;
                    case 24:
                        shipType = "Wing in ground (WIG), Hazardous category D";
                        break;
                    case 25:
                    case 26:
                    case 27:
                    case 28:
                    case 29:
                        shipType = "Wing in ground (WIG), Reserved for future use";
                        break;
                    case 30:
                        shipType = "Fishing";
                        break;
                    case 31:
                        shipType = "Towing";
                        break;
                    case 32:
                        shipType = "Towing: length exceeds 200m or breadth exceeds 25m";
                        break;
                    case 33:
                        shipType = "Dredging or underwater ops";
                        break;
                    case 34:
                        shipType = "Diving ops";
                        break;
                    case 35:
                        shipType = "Military ops";
                        break;
                    case 36:
                        shipType = "Sailing";
                        break;
                    case 37:
                        shipType = "Pleasure Craft";
                        break;
                    case 38:
                    case 39:
                        shipType = "Reserved";
                        break;
                    case 40:
                        shipType = "High speed craft(HSC), all ships of this type";
                        break;
                    case 41:
                        shipType = "High speed craft(HSC), Hazardous category A";
                        break;
                    case 42:
                        shipType = "High speed craft(HSC), Hazardous category B";
                        break;
                    case 43:
                        shipType = "High speed craft(HSC), Hazardous category C";
                        break;
                    case 44:
                        shipType = "High speed craft(HSC), Hazardous category D";
                        break;
                    case 45:
                    case 46:
                    case 47:
                    case 48:
                        shipType = "High speed craft(HSC), Reserved for future use";
                        break;
                    case 49:
                        shipType = "High speed craft(HSC), No additional information";
                        break;
                    case 50:
                        shipType = "Pilot Vessel";
                        break;
                    case 51:
                        shipType = "Search and Rescue vessel";
                        break;
                    case 52:
                        shipType = "Tug";
                        break;
                    case 53:
                        shipType = "Port Tender";
                        break;
                    case 54:
                        shipType = "Anti - pollution equipment";
                        break;
                    case 55:
                        shipType = "Law Enforcement";
                        break;
                    case 56:
                    case 57:
                        shipType = "Spare - Local Vessel";
                        break;
                    case 58:
                        shipType = "Medical Transport";
                        break;
                    case 59:
                        shipType = "Noncombatant ship according to RR Resolution No. 18";
                        break;
                    case 60:
                        shipType = "Passenger, all ships of this type";
                        break;
                    case 61:
                        shipType = "Passenger, Hazardous category A";
                        break;
                    case 62:
                        shipType = "Passenger, Hazardous category B";
                        break;
                    case 63:
                        shipType = "Passenger, Hazardous category C";
                        break;
                    case 64:
                        shipType = "Passenger, Hazardous category D";
                        break;
                    case 65:
                    case 66:
                    case 67:
                    case 68:
                        shipType = "Passenger, Reserved for future use";
                        break;
                    case 69:
                        shipType = "Passenger, No additional information";
                        break;
                    case 70:
                        shipType = "Cargo, all ships of this type";
                        break;
                    case 71:
                        shipType = "Cargo, Hazardous category A";
                        break;
                    case 72:
                        shipType = "Cargo, Hazardous category B";
                        break;
                    case 73:
                        shipType = "Cargo, Hazardous category C";
                        break;
                    case 74:
                        shipType = "Cargo, Hazardous category D";
                        break;
                    case 75:
                    case 76:
                    case 77:
                    case 78:
                        shipType = "Cargo, Reserved for future use";
                        break;
                    case 79:
                        shipType = "Cargo, No additional information";
                        break;
                    case 80:
                        shipType = "Tanker, all ships of this type";
                        break;
                    case 81:
                        shipType = "Tanker, Hazardous category A";
                        break;
                    case 82:
                        shipType = "Tanker, Hazardous category B";
                        break;
                    case 83:
                        shipType = "Tanker, Hazardous category C";
                        break;
                    case 84:
                        shipType = "Tanker, Hazardous category D";
                        break;
                    case 85:
                    case 86:
                    case 87:
                    case 88:
                        shipType = "Tanker, Reserved for future use";
                        break;
                    case 89:
                        shipType = "Tanker, No additional information";
                        break;
                    case 90:
                        shipType = "Other Type, all ships of this type";
                        break;
                    case 91:
                        shipType = "Other Type, Hazardous category A";
                        break;
                    case 92:
                        shipType = "Other Type, Hazardous category B";
                        break;
                    case 93:
                        shipType = "Other Type, Hazardous category C";
                        break;
                    case 94:
                        shipType = "Other Type, Hazardous category D";
                        break;
                    case 95:
                    case 96:
                    case 97:
                    case 98:
                        shipType = "Other Type, Reserved for future use";
                        break;
                    case 99:
                        shipType = "Other Type, no additional information";
                        break;
                    default:
                        shipType = "undefined";
                        break;
                       
                }
                return shipType;
            }
            catch (Exception)
            {
                throw;
            }            
        }
        public static string GetShipTypesCategory(int ShipId)
        {
            try
            {
                string shipType = "";
                switch (ShipId)
                {
                    case 0:                                                
                    case 1:
                    case 2:
                    case 3:
                    case 4:
                    case 5:
                    case 6:
                    case 7:
                    case 8:
                    case 9:
                    case 10:
                    case 11:
                    case 12:
                    case 13:
                    case 14:
                    case 15:
                    case 16:
                    case 17:
                    case 18:
                    case 19:
                    case 20:
                    case 21:
                    case 22:
                    case 23:
                    case 24:
                    case 25:
                    case 26:
                    case 27:
                    case 28:
                    case 29:
                    case 31:
                    case 32:
                    case 38:
                    case 39:
                    case 90:
                    case 91:
                    case 92:
                    case 93:
                    case 94:
                    case 95:
                    case 96:
                    case 97:
                    case 98:
                    case 99:
                        shipType = "Unspecified Ships";
                        break;                                        
                    case 30:
                        shipType = "Fishing";
                        break;                   
                    case 33:
                    case 34:
                    case 35:
                    case 50:
                    case 51:
                    case 52:
                    case 54:
                    case 55:
                    case 58:
                        shipType = "Tugs and Special Craft";
                        break;                    
                    case 36:
                    case 37:
                    case 53:
                    case 56:
                    case 57:
                    case 59:
                        shipType = "Pleasure Craft";
                        break;                   
                    case 40:
                    case 41:
                    case 42:
                    case 43:
                    case 44:
                    case 45:
                    case 46:
                    case 47:
                    case 48:
                    case 49:
                        shipType = "High Speed Craft";
                        break;                   
                    case 60:
                    case 61:
                    case 62:
                    case 63:
                    case 64:
                    case 65:
                    case 66:
                    case 67:
                    case 68:
                    case 69:                    
                        shipType = "Passenger Vessels";
                        break;
                    case 70:
                    case 71:
                    case 72:
                    case 73:
                    case 74:
                    case 75:
                    case 76:
                    case 77:
                    case 78:
                    case 79:
                        shipType = "Cargo Vessels";
                        break;
                        
                    case 80:
                    case 81:
                    case 82:
                    case 83:
                    case 84:
                    case 85:                    
                    case 86:
                    case 87:
                    case 88:
                    case 89:
                        shipType = "Tankers";
                        break;
                                      
                    default:
                        shipType = "Unsepcified Ships";
                        break;

                }
                return shipType;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
