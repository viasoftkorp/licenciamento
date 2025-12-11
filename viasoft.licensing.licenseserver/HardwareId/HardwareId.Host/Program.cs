using System;
using DeviceId;
using DeviceId.Encoders;
using DeviceId.Formatters;
using Viasoft.Licensing.LicenseServer.Shared.Services.HardwareId;

namespace HardwareId.Host
{
    public class Program
    {
        private const string HardwareIdNormal = "1";
        private const string HardwareIdSimple = "2";
        
        public static void Main(string[] args)
        {
            try
            {
                var option = AskToUserWhichConfigToUse();
                
                var simple = option == "2";
                
                var hardwareIdService = new ProvideHardwareIdService();
                var hardwareId = hardwareIdService.ProvideHardwareId(simple);

                Console.WriteLine($"HardwareId: {hardwareId}");
                
                if (simple)
                {
                    ShowSimpleComponents();
                }
                else
                {
                    ShowNormalComponents();
                }
                
                Console.WriteLine("Pressione ENTER para fechar");
                Console.ReadLine();
            }
            catch (Exception e)
            {
                Console.WriteLine("Houve um problema ao gerar o identificador {0}, {1}", e.Message, e);
            }
        }

        private static string AskToUserWhichConfigToUse()
        {
            var option = "";
            Console.WriteLine("Por favor informe o modelo de configuração de identificador de dispositivo");
            Console.WriteLine("{0} para normal", HardwareIdNormal);
            Console.WriteLine("{0} para simples (normalmente utilizado por máquinas hospedadas na nuvem)", HardwareIdSimple);
                
            while (true)
            {
                option = Console.ReadLine();
                if (string.IsNullOrEmpty(option) || (option.Trim() != "1" && option.Trim() != "2"))
                {
                    Console.WriteLine("As opções válidas são {0} ou {1}", HardwareIdNormal, HardwareIdSimple);
                }
                else
                {
                    break;
                }
            }

            return option.Trim();
        }

        private static void ShowNormalComponents()
        {
            var processorId = GetDeviceIdString(windowsBuilder => { windowsBuilder.AddProcessorId(); });
            var motherboardSerialNumber = GetDeviceIdString(windowsBuilder => { windowsBuilder.AddMotherboardSerialNumber(); });
            var systemSerialDriveNumber = GetDeviceIdString(windowsBuilder => { windowsBuilder.AddSystemSerialDriveNumber(); });
            var systemUuid = GetDeviceIdString(windowsBuilder => { windowsBuilder.AddSystemUuid(); });
            
            Console.WriteLine("Componentes utilizados");
            Console.WriteLine("Componente 1 {0}", processorId);
            Console.WriteLine("Componente 2 {0}", motherboardSerialNumber);
            Console.WriteLine("Componente 3 {0}", systemSerialDriveNumber);
            Console.WriteLine("Componente 4 {0}", systemUuid);
        }

        private static void ShowSimpleComponents()
        {
            var macAddressFromWmi = GetDeviceIdString(windowsBuilder => { windowsBuilder.AddMacAddressFromWmi(false, false); });
            var machineGuid = GetDeviceIdString(windowsBuilder => { windowsBuilder.AddMachineGuid(); });
            var systemUuid = GetDeviceIdString(windowsBuilder => { windowsBuilder.AddSystemUuid(); });
            
            Console.WriteLine("Componentes utilizados");
            Console.WriteLine("Componente 1 {0}", macAddressFromWmi);
            Console.WriteLine("Componente 2 {0}", machineGuid);
            Console.WriteLine("Componente 3 {0}", systemUuid);
        }

        private static string GetDeviceIdString(Action<WindowsDeviceIdBuilder> windowsBuilderConfiguration)
        {
            var result = new DeviceIdBuilder()
                .OnWindows(windowsBuilderConfiguration)
                .UseFormatter(new StringDeviceIdFormatter(new PlainTextDeviceIdComponentEncoder()))
                .ToString();
            return result;
        }
    }
}