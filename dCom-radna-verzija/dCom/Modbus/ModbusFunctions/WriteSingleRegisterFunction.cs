using Common;
using Modbus.FunctionParameters;
using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;

namespace Modbus.ModbusFunctions
{
    /// <summary>
    /// Class containing logic for parsing and packing modbus write single register functions/requests.
    /// </summary>
    public class WriteSingleRegisterFunction : ModbusFunction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WriteSingleRegisterFunction"/> class.
        /// </summary>
        /// <param name="commandParameters">The modbus command parameters.</param>
        public WriteSingleRegisterFunction(ModbusCommandParameters commandParameters) : base(commandParameters)
        {
            CheckArguments(MethodBase.GetCurrentMethod(), typeof(ModbusWriteCommandParameters));
        }

        /// <inheritdoc />
        public override byte[] PackRequest()
        {
            //TO DO: IMPLEMENT
            ModbusWriteCommandParameters ModbusWrite = this.CommandParameters as ModbusWriteCommandParameters;
            byte[] request = new byte[12];
            Buffer.BlockCopy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)ModbusWrite.TransactionId)), 0, request, 0, 2);
            Buffer.BlockCopy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)ModbusWrite.ProtocolId)), 0, request, 2, 2);
            Buffer.BlockCopy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)ModbusWrite.Length)), 0, request, 4, 2);
            request[6] = ModbusWrite.UnitId;
            request[7] = ModbusWrite.FunctionCode;
            Buffer.BlockCopy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)ModbusWrite.OutputAddress)), 0, request, 8, 2);
            Buffer.BlockCopy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)ModbusWrite.Value)), 0, request, 10, 2);
            return request;
        }

        /// <inheritdoc />
        public override Dictionary<Tuple<PointType, ushort>, ushort> ParseResponse(byte[] response)
        {
            //TO DO: IMPLEMENT
            ModbusWriteCommandParameters ModbusWrite = this.CommandParameters as ModbusWriteCommandParameters;//nas command parameters castujemo u write command parameters
            Dictionary<Tuple<PointType, ushort>, ushort> ret = new Dictionary<Tuple<PointType, ushort>, ushort>();

            var adresa = BitConverter.ToUInt16(response, 8);        //uzimamo adresu
            var vrednost = BitConverter.ToUInt16(response, 10);     //uzimamo vrednost

            adresa = (ushort)IPAddress.NetworkToHostOrder((short)adresa);           //pretvaramo sa mreznog oblika u normalan oblik
            vrednost = (ushort)IPAddress.NetworkToHostOrder((short)vrednost);

            ret.Add(new Tuple<PointType, ushort>(PointType.ANALOG_OUTPUT, adresa), vrednost);        //smestanje u recnik

            return ret;
        }
    }
}