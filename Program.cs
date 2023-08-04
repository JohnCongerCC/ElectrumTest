using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace ElectrumBalanceChecker
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // Bitcoin address to check the balance
            string address = "1LdRcdxfbSnmCYYNdeYpUnztiYzVfBEQeC";
            string scriptHash = "5437e7ed4cb286990c682c912fd9f111334ad767c055f59da3381541e9e761a6";
          
            var server = "mc.nerdmilio.com";
            var port = 50001;
            try
            {
                //balance for single address - ElectrumX
                using (var socket = new TcpClient(server, port)) 
                {
                    String json = "{\"id\": \"1\", \"method\": \"blockchain.scripthash.get_balance\", \"params\": [\"" + scriptHash + "\"], \"jsonrpc\" : \"1.0\"}\n";
                    var body = Encoding.UTF8.GetBytes(json);
                    using (var stream = socket.GetStream())
                    {
                        stream.Write(body, 0, body.Length);

                        byte[] bb = new byte[10000];
                        int k = stream.Read(bb, 0, 10000);
                        string resp = Encoding.UTF8.GetString(bb, 0, k);
                        if (!resp.Contains("result") || resp.Replace(" ", "").Contains("result\":null")) //if no result
                        {
                            if (string.IsNullOrEmpty(resp))
                                Console.WriteLine("Response from Getbalance is empty");
                            else
                                Console.WriteLine("Response from Getbalance is not what was expected:" + resp);
                        }
                        else
                        {
                            //{"jsonrpc": "2.0", "id": "1", "result": {"confirmed": 0, "unconfirmed": 0}}
                            int pos = resp.IndexOf("confirmed") + 11;
                            int pos2 = resp.IndexOf(',', pos);
                            string qq = resp.Substring(pos, pos2 - pos);
                            long q = long.Parse(qq);
                            double amt = (double)q / 100000000;

                            Console.WriteLine("GetBalanceSingle() address {0} server {1} port {2} is {3}", address, server, port, amt);
                            //Console.WriteLine(resp);
                        }
                    }
                }
            }
            catch (System.Net.Sockets.SocketException se)
            {
                Console.WriteLine("GetBalanceSingle() address {0} server {1} port {2}", address, server, port);
                Console.WriteLine(se.Message);
            }
        }
    }
}
