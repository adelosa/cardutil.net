using System.Text;
using Microsoft.Extensions.Configuration;

namespace Cardutil.Iso8583;

/// <summary>
/// Iso8583Message class represents a single ISO8583 message.
/// Messages are made up of a message type indicator (MTI) and 
/// message fields referred to as data elements (DE)
/// </summary>
public class Iso8583Message {

    public string Mti = "";
    public Dictionary<string, Iso8583Field> Fields = [];  
    private IConfiguration? _config;

    /// <summary>
    /// Constructor creates an empty Iso8583Message object 
    /// </summary>
    public Iso8583Message(){}
    
    /// <summary>
    /// Constructor creates an Iso8583Message object from a byte array.
    /// <list type="bullet">
    /// <item><description>Message Type indicator - 4 bytes</description></item>
    /// <item><description>Binary bitmap - 16 bytes (Reads DE1 - assume always present)</description></item>
    /// </list>
    /// </summary>
    /// <param name="inBytes">Bytes that contain the Iso8583 message</param>
    public Iso8583Message(byte[] inBytes, IConfiguration config, Encoding? encoding = null)
    {
        _config = config;
        encoding ??= System.Text.Encoding.ASCII;
        
        // hexdump full message
        Console.Write(Utils.HexDump(inBytes));
        
        // read and set Mti - always first 4 bytes 
        this.Mti = encoding.GetString(inBytes, 0, 4);
        Console.WriteLine($"Got MTI={this.Mti}");
        
        // read bitmap
        var bitmapView = inBytes.Skip(4).Take(16);
        if (bitmapView.Count() != 16) {
            Console.WriteLine("Invalid or no bitmap - finish");
            Console.WriteLine("*****************************");
            return;
        }
        byte[] bitmap = bitmapView.ToArray(); //new byte[16];
        Console.WriteLine("Following line is Bitmap");
        Console.Write(Utils.HexDump(bitmap));
        
        // convert bitmap to bool array
        bool[] bitmapArray = BitArray.getBits(bitmap);
        
        // parse the message fields based on bitmap
        int position = 20;  // MTI(4) + Bitmap(16)
        for (int bit=2; bit<bitmapArray.Length; bit++) {
            
            // skip if field not set
            if (!bitmapArray[bit])
                continue;

            // process the field
            Console.WriteLine($"processing bit {bit}...");
            string? bitType = config.GetValue<string>($"iso:{bit}:type");
            Console.WriteLine($"iso:{bit}:type = {bitType}");
            int bitLength = config.GetValue<int>($"iso:{bit}:length");
            Console.WriteLine($"iso:{bit}:length = {bitLength}");
            string value;
            Iso8583Field field = new();

            // Fixed fields
            switch(bitType)
            {
                case "FIXED":
                    value = encoding.GetString(inBytes, position, bitLength);
                    position += bitLength;
                    field.Key = bit.ToString();
                    field.StringValue = value;
                    this.Fields.Add(bit.ToString(), field);
                    break;
                case "LLVAR":
                case "LLLVAR":
                    // Override the length based on LL/LLL value
                    int llLength = 2;
                    if (bitType == "LLLVAR") llLength = 3;
                    bitLength = llLength + Int32.Parse(encoding.GetString(inBytes, position, llLength));
                    value = encoding.GetString(inBytes, position + llLength, bitLength - llLength);
                    position += bitLength;
                    field.Key = bit.ToString();
                    field.StringValue = value;
                    this.Fields.Add(bit.ToString(), field);
                    break;
                default:
                    value = "";
                    break;
            }
            Console.WriteLine($"DE{bit} = {value}");

        }
        Console.WriteLine("*****************************");
    }

    /// <summary>
    /// Converts an Iso8583Message object to bytes
    /// </summary>
    /// <returns>Bytes containing the Iso8583 message</returns>
    public byte[] ToBytes() {
        byte[] outputBytes = [];
        return outputBytes;
    }   
}

/// <summary>
/// Iso8583Field represents a single field in an Iso8583 record
/// </summary>
public struct Iso8583Field {
    public string Key {get; set;}
    public int IntValue {get; set;}
    public string StringValue {get; set;}
    public DateOnly DateValue {get; set;}
}