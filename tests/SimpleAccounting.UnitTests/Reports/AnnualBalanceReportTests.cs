﻿// <copyright>
//     Copyright (c) Lukas Grützmacher. All rights reserved.
// </copyright>

namespace lg2de.SimpleAccounting.UnitTests.Reports
{
    using System.Globalization;
    using System.Linq;
    using System.Xml.Linq;
    using System.Xml.XPath;
    using FluentAssertions;
    using FluentAssertions.Execution;
    using lg2de.SimpleAccounting.Model;
    using lg2de.SimpleAccounting.Reports;
    using lg2de.SimpleAccounting.UnitTests.Presentation;
    using Xunit;

    public class AnnualBalanceReportTests
    {
        [Fact]
        public void CreateReport_SampleData_Converted()
        {
            AccountingData project = Samples.SampleProject;
            project.Journal.Last().Booking.AddRange(Samples.SampleBookings);
            var setup = new AccountingDataSetup();
            AccountingDataJournal journal = project.Journal.Last();
            var sut = new AnnualBalanceReport(
                journal,
                project.AllAccounts,
                setup,
                project.Journal.Last().Year,
                new CultureInfo("en-us"));

            sut.CreateReport();

            var _ = new AssertionScope();

            sut.Document.XPathSelectElement("//text[@ID='saldo']")?.Value.Should().Be("150.00");

            var expected = @"
<data target=""income"">
  <tr>
    <td />
    <td>00400 Salary</td>
    <td>200.00</td>
    <td />
  </tr>
</data>";
            sut.Document.XPathSelectElement("//table/data[@target='income']")
                .Should().BeEquivalentTo(XDocument.Parse(expected).Root);

            expected = @"
<data target=""expense"">
  <tr>
    <td />
    <td>00600 Shoes</td>
    <td>-50.00</td>
    <td />
  </tr>
</data>";
            sut.Document.XPathSelectElement("//table/data[@target='expense']")
                .Should().BeEquivalentTo(XDocument.Parse(expected).Root);

            expected = @"
<data target=""receivable"" />";
            sut.Document.XPathSelectElement("//table/data[@target='receivable']")
                .Should().BeEquivalentTo(XDocument.Parse(expected).Root);

            expected = @"
<data target=""liability"">
  <tr>
    <td />
    <td>05000 Bank credit</td>
    <td>-2600.00</td>
    <td />
  </tr>
</data>";
            sut.Document.XPathSelectElement("//table/data[@target='liability']")
                .Should().BeEquivalentTo(XDocument.Parse(expected).Root);

            expected = @"
<data target=""asset"">
  <tr>
    <td />
    <td>00100 Bank account</td>
    <td>750.00</td>
    <td />
  </tr>
  <tr>
    <td />
    <td>Verbindlichkeiten</td>
    <td>-2600.00</td>
    <td />
  </tr>
</data>";
            sut.Document.XPathSelectElement("//table/data[@target='asset']")
                .Should().BeEquivalentTo(XDocument.Parse(expected).Root);
        }
    }
}