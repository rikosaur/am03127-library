///
/// Copyright (c) Rik Brugman 2017
/// 

using System;
using System.IO;
using System.IO.Ports;

namespace rikosaur.am03127_library
{
    public class Board
    {
        private string comPort;
        private int baudrate;
        private Parity parity;
        private Handshake handshake;
        private SerialPort serial;
        private int signId = 0;
        private LeadingAnimation leadingAnimation = LeadingAnimation.ScrollLeft;
        private EndingAnimation endingAnimation = EndingAnimation.Xopen;
        private Brightness brightness = Brightness.Brightest;
        private Page page = Page.A;
        private FontSize fontSize = FontSize.Normal;
        private Color color = Color.BrightRed;

        public Board(string comPort, int baudrate = 9600, Handshake handshake = Handshake.None,
            Parity parity = Parity.None)
        {
            // set serial options
            this.comPort = comPort;
            this.parity = parity;
            this.baudrate = baudrate;
            this.handshake = handshake;

            // create port handler
            serial = new SerialPort(this.comPort);

            serial.DataReceived += delegate(Object sender, SerialDataReceivedEventArgs e)
            {
                SerialPort sp = (SerialPort) sender;
                string indata = sp.ReadExisting();
                Console.WriteLine(indata);
            };


            // try to open port
            try
            {
                serial.Open();
            }
            catch (UnauthorizedAccessException e)
            {
                throw new UnauthorizedAccessException(e.Message, e.InnerException);
            }
            catch (ArgumentOutOfRangeException e)
            {
                throw new ArgumentOutOfRangeException(e.Message, e.InnerException);
            }
            catch (ArgumentException e)
            {
                throw new ArgumentException(e.Message, e.InnerException);
            }
            catch (IOException e)
            {
                throw new IOException(e.Message, e.InnerException);
            }
            catch (InvalidOperationException e)
            {
                throw new InvalidOperationException(e.Message, e.InnerException);
            }
        }

        public void sendMessage(string message)
        {
            // set string to upload
            string transferData = "";

            // set line
            transferData += "<L1>";

            // set page
            transferData += "<P" + (char) this.page + ">";

            // set leading animation
            transferData += "<F" + (char) this.leadingAnimation + ">";

            // set waiting animation
            transferData += "<MA>";

            // set waiting time before applying update
            transferData += "<WC>";

            // set ending animation
            transferData += "<F" + (char) this.endingAnimation + ">";

            // set font
            transferData += "<A" + (char) this.fontSize + ">";

            // set color
            transferData += "<C" + (char) this.color + ">";

            // set message
            transferData += message;

            // set XOR checksum
            transferData += calculateXor(transferData);

            // set ending tag
            transferData += "<E>";

            // send message to board
            serial.Write("<ID0" + this.signId + ">" + transferData);

        }

        public void setBrightness(Brightness brightness)
        {
            this.brightness = brightness;

            string brightnessCommand = "<B" + (char) brightness + ">";

            serial.Write("<ID0" + this.signId + ">" + brightnessCommand + calculateXor(brightnessCommand) + "<E>");
        }

        public void setPage(Page page)
        {
            this.page = page;
        }

        public void setColor(Color color)
        {
            this.color = color;
        }

        public void setLeadingAnimation(LeadingAnimation leadingAnimation)
        {
            this.leadingAnimation = leadingAnimation;
        }

        public void setEndingAnimation(EndingAnimation endingAnimation)
        {
            this.endingAnimation = endingAnimation;
        }

        private string calculateXor(string data)
        {
            Char[] t = data.ToCharArray();
            byte res = 0;
            for (int i = 0; i < t.Length; i++)
            {
                res ^= Convert.ToByte(t[i]);
            }
            return string.Format("{0:X2}", res);
        }
    }
}