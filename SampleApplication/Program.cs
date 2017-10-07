///
/// Copyright (c) Rik Brugman 2017
/// 

using System.Threading;
using rikosaur.am03127_library;

namespace SampleApplication
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            
            // connect
            Board ledBoard = new Board("COM7");
            
            // send the brightness setting to the board
            ledBoard.setBrightness(Brightness.Brightest);
            
            // do not block the serial connection by quitting the application
            Thread.Sleep(100);
            
            // send the message to the board, along with the text color
            ledBoard.setColor(Color.Green);
            ledBoard.sendMessage("You can connect me to a webservice, whoooo!");

            // do not block the serial connection by quitting the application
            Thread.Sleep(100);
            
        }
    }
}