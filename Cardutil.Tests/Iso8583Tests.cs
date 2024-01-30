using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;


namespace Cardutil.UnitTests
{    
    public class Iso8583Tests
    {
        private ConfigurationBuilder Builder = new();

        public Iso8583Tests() 
        {
            // Add ISO config here. Can be changes by specific tests if required.
            var json = """
            { 
                "iso": {
                    "2": {
                        "type": "FIXED",
                        "length": 5
                    },
                    "3": {
                        "type": "LLVAR",
                        "length": 0
                    },
                    "4": {
                        "type": "LLLVAR",
                        "length": 0
                    }
                }
            }
            """;
            this.Builder.AddJsonStream(new MemoryStream(Encoding.ASCII.GetBytes(json)));
        }

        [Fact]
        public void DummyTest()
        {
            Cardutil.Iso8583.Iso8583Message message = new();
            Assert.True(true);
        }
        
        [Fact]
        public void Iso8583MessageBasicFromBytesTestAscii()
        {
            byte[] inBytes = Encoding.ASCII.GetBytes("1234"); 
            Cardutil.Iso8583.Iso8583Message message = new(inBytes, this.Builder.Build());
            Assert.Equal("1234", message.Mti);
            Assert.Empty(message.Fields);
        }

        [Fact]
        public void Iso8583MessageBitmapFromBytesTestAscii()
        {
            byte[] mti = Encoding.ASCII.GetBytes("1234");
            byte[] bitmap = [0xf0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 
                             0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 ];
            byte[] de2 = Encoding.ASCII.GetBytes("12345");
            byte[] de3 = Encoding.ASCII.GetBytes("03123");
            byte[] de4 = Encoding.ASCII.GetBytes("003999");
            byte[] inBytes = [.. mti, .. bitmap, .. de2, .. de3, .. de4];
            
            Cardutil.Iso8583.Iso8583Message message = new(inBytes, this.Builder.Build());
            Assert.Equal("1234", message.Mti);
            Assert.Equal(3, message.Fields.Count);
            Console.WriteLine(message.Fields["2"].StringValue);
            Assert.Equal("12345", message.Fields["2"].StringValue);
            Console.WriteLine(message.Fields["3"].StringValue);
            Assert.Equal("123", message.Fields["3"].StringValue);
            Console.WriteLine(message.Fields["4"].StringValue);
            Assert.Equal("999", message.Fields["4"].StringValue);
        }

        [Fact]
        public void Iso8583MessageBasicFromBytesTestIBM500()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);  // get add'l codepages
            Encoding encoding = Encoding.GetEncoding("IBM500"); 
            byte[] inBytes = encoding.GetBytes("1234"); 
            Cardutil.Iso8583.Iso8583Message message = new(inBytes, this.Builder.Build(), encoding);
            Assert.Equal("1234", message.Mti);
            Assert.Empty(message.Fields);
        }
    }
}