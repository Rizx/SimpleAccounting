﻿// <copyright>
//     Copyright (c) Lukas Grützmacher. All rights reserved.
// </copyright>

namespace SimpleAccounting.UnitTests.Presentation
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using CsvHelper.Configuration;
    using FluentAssertions;
    using lg2de.SimpleAccounting.Model;
    using lg2de.SimpleAccounting.Presentation;
    using Xunit;

    public class ImportBookingsViewModelTests
    {
        [Fact]
        public void ImportBookings_SampleInput_DataImported()
        {
            var accounts = Samples.SampleProject.AllAccounts.ToList();
            var sut = new ImportBookingsViewModel(
                null,
                null,
                accounts);
            sut.SelectedAccount = accounts.Single(x => x.Name == "Bank account");
            sut.SelectedAccount.ImportMapping.Patterns = new List<AccountDefinitionImportMappingPattern>
            {
                new AccountDefinitionImportMappingPattern
                {
                    Expression = "Text1",
                    AccountID = 600
                }
            };
            sut.RangeMin = new DateTime(2019, 1, 1);
            sut.RangMax = new DateTime(2021, 1, 1);

            var input = @"
Date;Name;Text;Value
2018-12-31;NameIgnore;TextIgnore;12.34
2019-12-31;Name1;Text1;12.34
2020-01-01;Name2;Text2;-42.42";
            using (var inputStream = new StringReader(input))
            {
                sut.ImportBookings(inputStream, new Configuration { Delimiter = ";", CultureInfo = new CultureInfo("en-us") });
            }

            sut.ImportData.Should().BeEquivalentTo(
                new { Date = new DateTime(2019, 12, 31), Name = "Name1", Text = "Text1", Value = 12.34, RemoteAccount = new { ID = 600 } },
                new { Date = new DateTime(2020, 1, 1), Name = "Name2", Text = "Text2", Value = -42.42 });
        }
    }
}