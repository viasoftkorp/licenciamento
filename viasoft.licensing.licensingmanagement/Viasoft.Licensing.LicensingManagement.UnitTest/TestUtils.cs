using System;
using System.Collections.Generic;

namespace Viasoft.Licensing.LicensingManagement.UnitTest
{
    public class TestUtils
    {
        public static List<Guid> Guids => new List<Guid>
        {
            Guid.Parse("6B1FA7AB-D290-4A75-96BD-7E561AA7BA84"),
            Guid.Parse("B89150D8-9603-481D-AAC4-1C26C4544F56"),
            Guid.Parse("7757C843-FB73-4651-9D9F-7E3996301601"),
            Guid.Parse("9DDC4F92-C0A5-4F53-B4A7-33CDF62C5958"),
            Guid.Parse("089F1738-1B63-461D-8B5B-63C6D7AD16D6"),
            Guid.Parse("4303A98B-7C5B-4782-9A52-2825A0905227"),
            Guid.Parse("303AEC23-F80C-41C6-BD4A-7BB526BD5D11"),
            Guid.Parse("03710A52-A9AB-4933-8BF3-A6BE121B4CE7"),
            Guid.Parse("41B01E15-33AA-429B-9F7A-7FC99E1B03E8"),
            Guid.Parse("C666B290-D3D1-4DFF-8108-BFC61E4E973C"),
            Guid.Parse("C541ACA4-2B6F-44F6-A991-3B441B47E3E8"),
            Guid.Parse("B03C4009-4E80-4236-A3AB-A1CFAA6FFAB6"),
            Guid.Parse("D6BF1ECF-DE7F-43B2-8A1C-546C1E8E8EC1"),
            Guid.Parse("05D3C03B-4EEF-481B-84CF-2F5AD4B527FB"),
        };

        public static List<string> Names => new List<string>
        {
            "Encefalite",
            "Tuberculose",
            "Asma",
            "Sifilis",
            "Cancer",
            "COVID-19",
            "Gripe",
            "Meningite",
            "Laringite",
            "Orquite"
        };

        public static List<DateTime> Dates => new List<DateTime>
        {
            new DateTime(2020, 01, 23),
            new DateTime(2020, 02, 23),
            new DateTime(2020, 03, 23),
            new DateTime(2020, 04, 23),
            new DateTime(2020, 05, 23),
            new DateTime(2020, 06, 23),
            new DateTime(2020, 07, 23),
            new DateTime(2020, 08, 23),
            new DateTime(2020, 09, 23),
            new DateTime(2020, 10, 23),
            new DateTime(2020, 11, 23),
            new DateTime(2020, 12, 23),
        };

        public static List<string> CodeString => new List<string>
        {
            "0001",
            "0002",
            "0003",
            "0004",
            "0005",
            "0006",
            "0007",
            "0001",
            "0001",
            "0010",
            "0011",
        };

        public static List<string> Cidades => new List<string>
        {
            "Curitiba",
            "Florianópolis",
            "Porto Alegre",
            "São Paulo",
            "Rio de Janeiro",
            "Belo Horizonte",
            "Vitória",
            "Belém",
            "Rio Branco",
            "Aracaju",
            "Recife",
        };

        public static List<string> Ruas => new List<string>
        {
            "Rua da Santa Fé",
            "Rua Dolores",
            "Avenida Afonso Pena",
            "Rua Serra de Bragança",
            "Rua Barão de Vitória",
            "Rua Cristiano Olsen",
            "Rua Paracatu",
            "Rodovia Raposo Tavares",
            "Avenida Almirante Maximiano Fonseca",
            "Rua Tenente-Coronel Cardoso",
            "Rua Arlindo Nogueira"
        };

        public static List<string> Ceps => new List<string>
        {
            "50980662",
            "69918632",
            "59122185",
            "41650015",
            "29902090",
            "59607548",
            "64207313",
            "58806622",
            "64018320",
            "29156593",
            "41207609"
        };

        public static List<string> Estados => new List<string>
        {
            "01",
            "02",
            "03",
            "04",
            "05",
            "06",
            "07",
            "01",
            "01",
            "10",
            "11",
        };

        public static List<int> CodeInt => new List<int>
        {
            1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11
        };

        public static List<decimal> Decimals => new List<decimal>
        {
            new decimal(10.7),
            new decimal(20.6),
            new decimal(30.5),
            new decimal(40.4),
            new decimal(50.3),
            new decimal(60.2),
            new decimal(70.1),
            new decimal(800.3),
            new decimal(900.2),
            new decimal(1000.1),
            new decimal(20500.1),
            new decimal(151000.1),
            new decimal(223390.1),
            new decimal(212.1),
        };
    }
}