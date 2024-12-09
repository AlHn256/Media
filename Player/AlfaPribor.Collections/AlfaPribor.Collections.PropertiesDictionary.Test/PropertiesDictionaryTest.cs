using AlfaPribor.Collections;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace AlfaPribor.Collections.PropertiesDictionaryTest
{
    /// <summary>Это класс теста для PropertiesDictionaryTest,
    /// в котором должны находиться все модульные тесты PropertiesDictionaryTest
    ///</summary>
    [TestClass()]
    public class PropertiesDictionaryTest
    {
        private TestContext testContextInstance;

        /// <summary>Получает или устанавливает контекст теста, в котором предоставляются
        ///сведения о текущем тестовом запуске и обеспечивается его функциональность.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Дополнительные атрибуты теста
        // 
        //При написании тестов можно использовать следующие дополнительные атрибуты:
        //
        //ClassInitialize используется для выполнения кода до запуска первого теста в классе
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //ClassCleanup используется для выполнения кода после завершения работы всех тестов в классе
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //TestInitialize используется для выполнения кода перед запуском каждого теста
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //TestCleanup используется для выполнения кода после завершения каждого теста
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        /// <summary>Тест для Separator</summary>
        [TestMethod()]
        public void SeparatorTest()
        {
            PropertiesDictionary target = new PropertiesDictionary();
            char expected = ',';
            char actual;
            target.Separator = expected;
            actual = target.Separator;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>Тест для ToString</summary>
        [TestMethod()]
        public void ToStringTest()
        {
            PropertiesDictionary target = new PropertiesDictionary();
            string expected = "a=1; b=2; c=3";
            string actual;
            target.Add("a", "1");
            target.Add("b", "2");
            target.Add("c", "3");
            actual = target.ToString();
            Assert.AreEqual(expected, actual);

            target.Add("d", "");
            expected += "; d";
            actual = target.ToString();
            Assert.AreEqual(expected, actual);
        }

        /// <summary>Тест для Parse</summary>
        [TestMethod()]
        public void ParseTest()
        {
            PropertiesDictionary target = new PropertiesDictionary();
            string properties = "a=1; b ; c = 3";
            try
            {
                target.Parse(properties);
            }
            catch
            {
                Assert.Inconclusive("Метод не должен генерировать исключения.");
            }
            string expected = "a=1; b; c=3";
            string actual;
            actual = target.ToString();
            Assert.AreEqual(expected, actual);
        }
    }
}