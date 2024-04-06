using Common;
using Modbus.FunctionParameters;
using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;

namespace Modbus.ModbusFunctions
{
    /// <summary>
    /// Class containing logic for parsing and packing modbus write coil functions/requests.
    /// </summary>
    public class WriteSingleCoilFunction : ModbusFunction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WriteSingleCoilFunction"/> class.
        /// </summary>
        /// <param name="commandParameters">The modbus command parameters.</param>
        public WriteSingleCoilFunction(ModbusCommandParameters commandParameters) : base(commandParameters)
        {
            CheckArguments(MethodBase.GetCurrentMethod(), typeof(ModbusWriteCommandParameters));
        }

        /// <inheritdoc />
        public override byte[] PackRequest()
        {
            //TO DO: IMPLEMENT
            ModbusWriteCommandParameters modbusWrite = this.CommandParameters as ModbusWriteCommandParameters;
            byte[] request = new byte[12];
            Buffer.BlockCopy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)modbusWrite.TransactionId)), 0, request, 0, 2);
            Buffer.BlockCopy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)modbusWrite.ProtocolId)), 0, request, 2, 2);
            Buffer.BlockCopy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)modbusWrite.Length)), 0, request, 4, 2);
            request[6] = modbusWrite.UnitId;
            request[7] = modbusWrite.FunctionCode;
            Buffer.BlockCopy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)modbusWrite.OutputAddress)), 0, request, 8, 2);
            Buffer.BlockCopy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)modbusWrite.Value)), 0, request, 10, 2);


            return request;
        }

        /// <inheritdoc />
        public override Dictionary<Tuple<PointType, ushort>, ushort> ParseResponse(byte[] response)
        {
            Dictionary<Tuple<PointType, ushort>, ushort> ret = new Dictionary<Tuple<PointType, ushort>, ushort>();
            short adresa = BitConverter.ToInt16(response, 8);
            short vrednost = BitConverter.ToInt16(response, 10);

            adresa = IPAddress.NetworkToHostOrder(adresa);
            vrednost = IPAddress.NetworkToHostOrder(vrednost);

            ret.Add(new Tuple<PointType, ushort>(PointType.DIGITAL_OUTPUT, (ushort)adresa), (ushort)vrednost);


            return ret;

        }
    }
}