using Common;
using Modbus.FunctionParameters;
using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;

namespace Modbus.ModbusFunctions
{
    /// <summary>
    /// Class containing logic for parsing and packing modbus read coil functions/requests.
    /// </summary>
    public class ReadCoilsFunction : ModbusFunction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReadCoilsFunction"/> class.
        /// </summary>
        /// <param name="commandParameters">The modbus command parameters.</param>
		public ReadCoilsFunction(ModbusCommandParameters commandParameters) : base(commandParameters)
        {
            CheckArguments(MethodBase.GetCurrentMethod(), typeof(ModbusReadCommandParameters));
        }

        /// <inheritdoc/>
        // Read Coils je Digitalan izlaz, digitalan to znaci da ima 1 bit

        public override byte[] PackRequest()
        {
            //TO DO: IMPLEMENT
            byte[] zahtev = new byte[12];

            Buffer.BlockCopy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)CommandParameters.TransactionId)), 0, zahtev, 0, 2);
            Buffer.BlockCopy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)CommandParameters.ProtocolId)), 0, zahtev, 2, 2);
            Buffer.BlockCopy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)CommandParameters.Length)), 0, zahtev, 4, 2);
            zahtev[6] = CommandParameters.UnitId;           //ne trebo hton jer je samo 1 byte
            zahtev[7] = CommandParameters.FunctionCode;

            Buffer.BlockCopy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)((ModbusReadCommandParameters)CommandParameters).StartAddress)), 0, zahtev, 8, 2);
            Buffer.BlockCopy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)((ModbusReadCommandParameters)CommandParameters).Quantity)), 0, zahtev, 10, 2);


            return zahtev;
        }

        /// <inheritdoc />
        public override Dictionary<Tuple<PointType, ushort>, ushort> ParseResponse(byte[] response)
        {
            //TO DO: IMPLEMENT

            var odgovor = new Dictionary<Tuple<PointType, ushort>, ushort>();
            // response[7] - FunctionCode
            // response[8] - ByteCount - kolko ima bytova vraca


            if (response[7] == CommandParameters.FunctionCode + 0x80)
            {
                HandeException(response[8]);
            }
            else
            {
                int brojac = 0;
                ushort adresa = ((ModbusReadCommandParameters)CommandParameters).StartAddress;
                ushort vrednost = 0;
                byte maska = 1;

                // posto je digitalno onda valjda je u bitima pa sad trazimo te bite tako da nam treba 2 fora

                for (int i = 0; i < response[8]; i++)               //prvi for da citamo po bytovima respones    response[8] - bytecount kolko byteova vraca
                {
                    byte temp = response[9 + i]; // od 9 krece data deo poruke
                    // 1110011 & 00000001
                    vrednost = (ushort)(temp & maska);
                    temp >>= 1;
                    //DIGITAL_OUTPUT obavezno
                    odgovor.Add(new Tuple<PointType, ushort>(PointType.DIGITAL_OUTPUT, adresa), vrednost);


                    brojac++;
                    adresa++;

                    if (brojac > ((ModbusReadCommandParameters)CommandParameters).Quantity)          ///quantity valjda oznacava broj bitova i onda posto mi mozemo imati nzm 4 bytea ali taj 4 byte ne mora biti popunjen do kraja sa bitovima pa pazimo da ne izadjmeo iz opsega
                    {
                        break;
                    }

                }



            }

            return odgovor;
        }
    }
}