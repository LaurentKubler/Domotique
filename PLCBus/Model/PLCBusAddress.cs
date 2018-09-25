using System;
using System.Text;

namespace PLCBus.Model
{
    public class PLCBusAddress
    {
        string HomeUnit { get; set; }

        string UserCode { get; set; }


        public PLCBusAddress(string address)
        {
            UserCode = address.Substring(0, 2);
            HomeUnit = address.Substring(2, 2);
        }


        private byte ToHomeUnitCode(String strCode)
        {
            byte val = 0;
            switch (strCode[0])
            {
                case 'A': val = 0; break;
                case 'B': val = 1; break;
                case 'C': val = 2; break;
                case 'D': val = 3; break;
                case 'E': val = 4; break;
                case 'F': val = 5; break;
                case 'G': val = 6; break;
                case 'H': val = 7; break;
                case 'I': val = 8; break;
                case 'J': val = 9; break;
                case 'K': val = 10; break;

            }
            val = (byte)(val * (byte)4);
            switch (strCode[1])
            {
                case '1': val += 0; break;
                case '2': val += 1; break;
                case '3': val += 2; break;
                case '4': val += 3; break;
                case '5': val += 4; break;
                case '6': val += 5; break;
                case '7': val += 6; break;
                case '8': val += 7; break;
                case '9': val += 8; break;
            }
            return val;
        }


        public static byte[] HexStringToByteArray(String s)
        {
            var byteArray = Encoding.ASCII.GetBytes(s);
            return byteArray;
        }


        public String ToStringHomeUnit(byte code)
        {
            String val = "";
            int home = code << 4;
            int unit = code & 0x0F;
            switch (home)
            {
                case 0: val = "A"; break;
                case 1: val = "B"; break;
                case 2: val = "C"; break;
                case 3: val = "D"; break;
                case 4: val = "E"; break;
                case 5: val = "F"; break;
                case 6: val = "G"; break;
                case 7: val = "H"; break;
                case 8: val = "I"; break;
                case 9: val = "J"; break;
                case 10: val = "K"; break;
            }
            switch (unit)
            {
                case 0: val += "1"; break;
                case 1: val += "2"; break;
                case 2: val += "3"; break;
                case 3: val += "4"; break;
                case 4: val += "5"; break;
                case 5: val += "6"; break;
                case 6: val += "7"; break;
                case 7: val += "8"; break;
                case 8: val += "9"; break;
                case 9: val += "10"; break;
                case 10: val += "11"; break;
            }
            return val;
        }
    }
}
