﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using LISMessageProcessor.Category;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RCL.LISConnector.DataEntity.IOT;
using RCL.LISConnector.DataEntity.SQL;
using RCL.LISConnector.HL7Parser;

namespace LISMessageProcessor.Test.CategoryA
{
    [TestClass]
    public class HL7
    {
        private HL7DecoderA _decoder;
        private HL7MessageProcessor _processor;
        private Message _message;
        private DeviceMessage _deviceMessage;
        private const string SendingFacility = "Test_Sending_Facility";
        private const string ClientId = "1234";

        public HL7()
        {
            _decoder = new HL7DecoderA();
            string strMessage = Helpers.GetStringFromFile(@"CategoryA\SampleMessages\HL7.txt");
            _message = new Message(strMessage);
            _message.ParseMessage();
            string strEncodedMessage = $"{(char)0x0b}{strMessage.Replace("\r\n", "\r")}{(char)0x1c}{(char)0x0d}";
            _deviceMessage = new DeviceMessage
            {
                ClientId = ClientId,
                DeviceCategory = "A",
                SendingFacility = SendingFacility,
                MessageType = "HL7",
                ContentsList = new List<string> { strEncodedMessage }
            };
            _processor = new HL7MessageProcessor(_decoder);
        }

        #region Decoder

        [TestMethod]
        public void GetPatient()
        {
            bool b = false;

            Patient patient = _decoder.GetPatient(_message);
            
            try
            {
                if (patient?.InternalPatientId != "0003")
                    Assert.Fail();
                if (patient?.FamilyName != "Fab")
                    Assert.Fail();
                if (patient?.GivenName != "Cesc")
                    Assert.Fail();
                if (patient?.DateOfBirth.Value.ToString("yyyyMMdd") != "19890811")
                    Assert.Fail();
                if (patient?.Sex != "M")
                    Assert.Fail();
                if (patient?.AccountNumber != "V003")
                    Assert.Fail();

                b = true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }

            Assert.IsTrue(b);
        }

        [TestMethod]
        public void GetDiagnosticReport()
        {
            bool b = false;

            DiagnosticReport report = _decoder.GetDiagnosticReport(_message);
            
            try
            {
                if (report?.SendingApplication != "cobasIT1000")
                    Assert.Fail();
                if (report?.PatientInternalId != "0003")
                    Assert.Fail();
                if (report?.FamilyName != "Fab")
                    Assert.Fail();
                if (report?.GivenName != "Cesc")
                    Assert.Fail();
                if (report?.Sex != "M")
                    Assert.Fail();
                if (report?.DateOfBirth.Value.ToString("yyyyMMdd") != "19890811")
                    Assert.Fail();
                if (report?.AnalyzerName != "ACI II UU13013667")
                    Assert.Fail();
                if (report?.AnalyzerDateTime.Value.ToString("yyyyMMddHHmmss") != "20130514114122")
                    Assert.Fail();
                if (report?.OperatorId != "ROCHE")
                    Assert.Fail();

                b = true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

            Assert.IsTrue(b);

        }

        [TestMethod]
        public void GetResults()
        {
            bool b = false;

            List<Result> _results = _decoder.GetResults(_message);
            Result _result = _results[0];
           
            try
            {
                if (_result?.TestCode != "Glu2")
                    Assert.Fail();
                if (_result?.Value != "67")
                    Assert.Fail();
                if (_result?.Units != "mg/dL")
                    Assert.Fail();
                if (_result?.ReferenceRange != "-")
                    Assert.Fail();
                if (_result?.ResultDateTime.Value.ToString("yyyyMMddHHmmss") != "20130514114122")
                    Assert.Fail();
                if (_result?.Comments != "Doctor Notified")
                    Assert.Fail();

                b = true;
            }

            catch(Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

            Assert.IsTrue(b);
        }

        #endregion

        #region Processor

        [TestMethod]
        public void ProcessMessage()
        {
            bool b = false;

            List<PatientDiagnosticRecord> _diagnosticRecords = _processor.ProcessMessage(_deviceMessage);
            PatientDiagnosticRecord _diagnosticRecord = _diagnosticRecords[0];

            try
            {
                Patient patient = _diagnosticRecord.patient;

                if (patient?.InternalPatientId != "0003")
                    Assert.Fail();
                if (patient?.FamilyName != "Fab")
                    Assert.Fail();
                if (patient?.GivenName != "Cesc")
                    Assert.Fail();
                if (patient?.DateOfBirth.Value.ToString("yyyyMMdd") != "19890811")
                    Assert.Fail();
                if (patient?.Sex != "M")
                    Assert.Fail();
                if (patient?.AccountNumber != "V003")
                    Assert.Fail();
                if (patient?.ClientId != ClientId)
                    Assert.Fail();

                DiagnosticReport report = _diagnosticRecord.diagnosticReport;

                if (report?.SendingApplication != "cobasIT1000")
                    Assert.Fail();
                if (report?.ReceivingApplication != LISMessageProcessor.Helpers.Constants.ReceivingApplicationName)
                    Assert.Fail();
                if (report?.ReceivingFacility != LISMessageProcessor.Helpers.Constants.ReceivingFacility)
                    Assert.Fail();
                if (report?.PatientInternalId != "0003")
                    Assert.Fail();
                if (report?.FamilyName != "Fab")
                    Assert.Fail();
                if (report?.GivenName != "Cesc")
                    Assert.Fail();
                if (report?.Sex != "M")
                    Assert.Fail();
                if (report?.DateOfBirth.Value.ToString("yyyyMMdd") != "19890811")
                    Assert.Fail();
                if (report?.AnalyzerName != "ACI II UU13013667")
                    Assert.Fail();
                if (report?.AnalyzerDateTime.Value.ToString("yyyyMMddHHmmss") != "20130514114122")
                    Assert.Fail();
                if (report?.OperatorId != "ROCHE")
                    Assert.Fail();
                if (report?.TestCodes != "Glu2")
                    Assert.Fail();
                if (report?.ClientId != ClientId)
                    Assert.Fail();

                List<Result> _results = _diagnosticRecord.results;
                Result _result = _results[0];

                if (_result?.TestCode != "Glu2")
                    Assert.Fail();
                if (_result?.Value != "67")
                    Assert.Fail();
                if (_result?.Units != "mg/dL")
                    Assert.Fail();
                if (_result?.ReferenceRange != "-")
                    Assert.Fail();
                if (_result?.ResultDateTime.Value.ToString("yyyyMMddHHmmss") != "20130514114122")
                    Assert.Fail();
                if (_result?.Comments != "Doctor Notified")
                    Assert.Fail();
                if (_result?.ClientId != ClientId)
                    Assert.Fail();

                b = true;

            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

            Assert.IsTrue(b);

        }

        #endregion
    }
}
