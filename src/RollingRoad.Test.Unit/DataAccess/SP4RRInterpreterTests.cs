using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using NSubstitute;
using NUnit.Framework;
using RollingRoad.Infrastructure.DataAccess;

namespace RollingRoad.Test.Unit.DataAccess
{
    [TestFixture]
    // ReSharper disable once InconsistentNaming
    public class SP4RRInterpreterTests
    {
        private MemoryStream _ms;
        private StreamWriter _writer;
        private StreamReader _reader;
        private SP4RRInterpreter _interpreter;
        private readonly CultureInfo _cultureTarget = new CultureInfo("en-US");

        [SetUp]
        public void SetUp()
        {
            _ms = new MemoryStream();
            _reader = new StreamReader(_ms);
            _writer = new StreamWriter(_ms);
            _interpreter = new SP4RRInterpreter(_reader, _writer);
        }

        [TearDown]
        public void TearDown()
        {
            _ms.Dispose();
        }

        private void WriteToMemoryStream(StreamWriter writer, string value)
        {
            _ms.SetLength(0);
            writer.Write(value);
            writer.Flush();
            _ms.Position = 0;
            writer.Flush();
        }

        [Test]
        public void Start_FirstStart_HandShakeSent()
        {
            _interpreter.Start();

            Assert.That(Encoding.ASCII.GetString(_ms.ToArray()), Is.EqualTo("0 RollingRoad\n"));
        }

        [Test]
        public void Stop_StartedAndStopped_StopSent()
        {
            _interpreter.Start();
            //Reset memory stream
            _ms.SetLength(0);
            _ms.Position = 0;

            _interpreter.Stop();

            Assert.That(Encoding.ASCII.GetString(_ms.ToArray()), Is.EqualTo("2\n"));
        }


        [TestCase(5.0)]
        [TestCase(-5.0)]
        [TestCase(0.0)]
        [TestCase(0.852)]
        [TestCase(-0.852)]
        public void SetTorque_StartedAndTorqueSet_TorqueSent(double value)
        {
            _interpreter.Start();
            //Reset memory stream
            _ms.SetLength(0);
            _ms.Position = 0;

            _interpreter.SetTorque(value);

            Assert.That(Encoding.UTF8.GetString(_ms.ToArray()), Is.EqualTo("4 " + value.ToString(_cultureTarget) + "\n"));
        }

        [TestCase(5.0)]
        [TestCase(-5.0)]
        [TestCase(0.0)]
        [TestCase(0.852)]
        [TestCase(-0.852)]
        public void Kp_SetValue_ValueSent(double value)
        {
            _interpreter.Start();
            //Reset memory stream
            _ms.SetLength(0);
            _ms.Position = 0;

            _interpreter.Kp = value;

            Assert.That(Encoding.UTF8.GetString(_ms.ToArray()), Is.EqualTo("5 " + value.ToString(_cultureTarget) + " 0 0\n"));
        }

        [TestCase(5.0)]
        [TestCase(-5.0)]
        [TestCase(0.0)]
        [TestCase(0.852)]
        [TestCase(-0.852)]
        public void Ki_SetValue_ValueSent(double value)
        {
            _interpreter.Start();
            //Reset memory stream
            _ms.SetLength(0);
            _ms.Position = 0;

            _interpreter.Ki = value;

            Assert.That(Encoding.UTF8.GetString(_ms.ToArray()), Is.EqualTo("5 0 " + value.ToString(_cultureTarget) + " 0\n"));
        }

        [TestCase(5.0)]
        [TestCase(-5.0)]
        [TestCase(0.0)]
        [TestCase(0.852)]
        [TestCase(-0.852)]
        public void Kd_SetValue_ValueSent(double value)
        {
            _interpreter.Start();
            //Reset memory stream
            _ms.SetLength(0);
            _ms.Position = 0;

            _interpreter.Kd = value;

            Assert.That(Encoding.UTF8.GetString(_ms.ToArray()), Is.EqualTo("5 0 0 " + value.ToString(_cultureTarget) + "\n"));
        }

        [TestCase(5.0)]
        [TestCase(-5.0)]
        [TestCase(0.0)]
        [TestCase(0.852)]
        [TestCase(-0.852)]
        public void OnNextReadValueEvent_OneUnitAndOneDataPoint_EventCalledWithCorrectData(double value)
        {
            StreamWriter writer = new StreamWriter(_ms);
            double valueRead = 0;

            _interpreter.OnNextReadValue += (sender, args) => valueRead = args.Data.First().Item1.Value;

            _interpreter.Start(false);

            WriteToMemoryStream(writer, "1 0 Time Seconds\n");
            _interpreter.Listen();
            
            WriteToMemoryStream( writer, $"3 {value.ToString(_cultureTarget)}\n");
            _interpreter.Listen();

            Assert.That(valueRead, Is.EqualTo(value));
        }

        [TestCase(5.0)]
        [TestCase(-5.0)]
        [TestCase(0.0)]
        [TestCase(0.852)]
        [TestCase(-0.852)]
        public void OnNextReadValueEvent_OneUnitAndTwoDataPoints_EventCalledWithCorrectDataEndPoints(double value)
        {
            StreamWriter writer = new StreamWriter(_ms);
            double valueRead = 0;

            _interpreter.OnNextReadValue += (sender, args) => valueRead = args.Data.First().Item1.Value;

            _interpreter.Start(false);

            WriteToMemoryStream(writer, "1 0 Time Seconds\n");
            _interpreter.Listen();

            WriteToMemoryStream(writer, "3 4.25\n");
            _interpreter.Listen();

            WriteToMemoryStream(writer, $"3 {value.ToString(_cultureTarget)}\n");
            _interpreter.Listen();

            Assert.That(valueRead, Is.EqualTo(value));
        }

        [Test]
        public void Start_StartStopStart_StartSent()
        {
            _interpreter.Start(true);
            _interpreter.Stop();

            _ms.SetLength(0);
            _interpreter.Start(true);

            Assert.That(Encoding.ASCII.GetString(_ms.ToArray()), Is.EqualTo("0 RollingRoad\n"));
        }

        [Test]
        public void Calibrate_CallCalibrate_CalibrateSent()
        {
            _interpreter.Calibrate();

            Assert.That(Encoding.ASCII.GetString(_ms.ToArray()), Is.EqualTo("6\n"));
        }

        [TestCase(5.0)]
        [TestCase(-5.0)]
        [TestCase(0.0)]
        [TestCase(0.852)]
        [TestCase(-0.852)]
        public void Kp_SendPIDValue_ValueSet(double value)
        {
            StreamWriter writer = new StreamWriter(_ms);

            _interpreter.Start(false);

            WriteToMemoryStream(writer, $"5 {value.ToString(_cultureTarget)} 4.2 5.2\n");
            _interpreter.Listen();

            Assert.That(_interpreter.Kp, Is.EqualTo(value).Within(double.Epsilon));
        }

        [TestCase(5.0)]
        [TestCase(-5.0)]
        [TestCase(0.0)]
        [TestCase(0.852)]
        [TestCase(-0.852)]
        public void Ki_SendPIDValue_ValueSet(double value)
        {
            StreamWriter writer = new StreamWriter(_ms);

            _interpreter.Start(false);

            WriteToMemoryStream(writer, $"5 3.2 {value.ToString(_cultureTarget)} 5.2\n");
            _interpreter.Listen();

            Assert.That(_interpreter.Ki, Is.EqualTo(value).Within(double.Epsilon));
        }

        [TestCase(5.0)]
        [TestCase(-5.0)]
        [TestCase(0.0)]
        [TestCase(0.852)]
        [TestCase(-0.852)]
        public void Kd_SendPIDValue_KpValueSet(double value)
        {
            StreamWriter writer = new StreamWriter(_ms);

            _interpreter.Start(false);

            WriteToMemoryStream(writer, $"5 3.2 4.2 {value.ToString(_cultureTarget)}\n");
            _interpreter.Listen();

            Assert.That(_interpreter.Kd, Is.EqualTo(value).Within(double.Epsilon));
        }
    }
}
