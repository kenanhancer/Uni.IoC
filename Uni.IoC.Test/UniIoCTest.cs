using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq.Expressions;
using Uni.IoC;

namespace Uni.IoC.Test
{
    [TestClass]
    public class UniIoCTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            UniIoC container = new UniIoC();

            container.Register(ServiceCriteria.For<ILoginService>().ImplementedBy<EmailLoginService>().Named("Email").Dependencies(new { loginValidator = new EmailLoginValidator() }));
            container.Register(ServiceCriteria.For<ILoginService>().ImplementedBy<PhoneLoginService>().Named("Phone").Dependencies(new { loginValidator = new PhoneLoginValidator() }));

            PhoneLoginService phoneLoginService = container.Resolve<PhoneLoginService>("Phone");
            ILoginService emailLoginService = container.Resolve<ILoginService>("Email");

            Assert.IsNotNull(phoneLoginService);
            Assert.IsNotNull(emailLoginService);

            string sessionKey1 = phoneLoginService.Login("", "");
            string sessionKey2 = emailLoginService.Login("", "");

            Assert.IsFalse(String.IsNullOrEmpty(sessionKey1));
            Assert.IsFalse(String.IsNullOrEmpty(sessionKey2));
        }

        [TestMethod]
        public void TestMethod2()
        {
            UniIoC container = new UniIoC();

            container.Register(ServiceCriteria.For<ILoginValidator>().ImplementedBy<EmailLoginValidator>().Named("EmailLoginValidator"));
            container.Register(ServiceCriteria.For<ILoginValidator>().ImplementedBy<PhoneLoginValidator>().Named("PhoneLoginValidator"));

            IDictionary<string, object> argumentsEmail = new Dictionary<string, object> { { "loginValidator", new EmailLoginValidator() } };
            IDictionary<string, object> argumentsPhone = new Dictionary<string, object> { { "loginValidator", new PhoneLoginValidator() } };

            container.Register(ServiceCriteria.For<ILoginService>().ImplementedBy<EmailLoginService>().Named("Email").Dependencies(argumentsEmail));
            container.Register(ServiceCriteria.For<ILoginService>().ImplementedBy<PhoneLoginService>().Named("Phone").Dependencies(argumentsPhone));

            PhoneLoginService phoneLoginService = container.Resolve<PhoneLoginService>("Phone");
            ILoginService emailLoginService = container.Resolve<ILoginService>("Email");

            Assert.IsNotNull(phoneLoginService);
            Assert.IsNotNull(emailLoginService);

            string sessionKey1 = phoneLoginService.Login("", "");
            string sessionKey2 = emailLoginService.Login("", "");

            Assert.IsFalse(String.IsNullOrEmpty(sessionKey1));
            Assert.IsFalse(String.IsNullOrEmpty(sessionKey2));
        }

        [TestMethod]
        public void TestMethod3()
        {
            UniIoC container = new UniIoC();

            container.Register(ServiceCriteria.For<ILoginValidator>().ImplementedBy<EmailLoginValidator>().Named("EmailLoginValidator"));
            container.Register(ServiceCriteria.For<ILoginValidator>().ImplementedBy<PhoneLoginValidator>().Named("PhoneLoginValidator"));

            container.Register(ServiceCriteria.For<ILoginService>().ImplementedBy<EmailLoginService>().Named("Email").Dependencies(new { loginValidator = container.Resolve<ILoginValidator>("EmailLoginValidator") }));
            container.Register(ServiceCriteria.For<ILoginService>().ImplementedBy<PhoneLoginService>().Named("Phone").Dependencies(new { loginValidator = container.Resolve<ILoginValidator>("PhoneLoginValidator") }));

            PhoneLoginService phoneLoginService = container.Resolve<PhoneLoginService>("Phone");
            ILoginService emailLoginService = container.Resolve<ILoginService>("Email");

            Assert.IsNotNull(phoneLoginService);
            Assert.IsNotNull(emailLoginService);

            string sessionKey1 = phoneLoginService.Login("", "");
            string sessionKey2 = emailLoginService.Login("", "");

            Assert.IsFalse(String.IsNullOrEmpty(sessionKey1));
            Assert.IsFalse(String.IsNullOrEmpty(sessionKey2));
        }

        [TestMethod]
        public void TestMethod4()
        {
            UniIoC container = new UniIoC();

            ILoginValidator emailLoginValidator = new EmailLoginValidator();
            ILoginValidator phoneLoginValidator = new PhoneLoginValidator();

            container.Register(ServiceCriteria.For<ILoginValidator>().ImplementedBy<EmailLoginValidator>().Named("EmailLoginValidator").Instance(emailLoginValidator));
            container.Register(ServiceCriteria.For<ILoginValidator>().ImplementedBy<PhoneLoginValidator>().Named("PhoneLoginValidator").Instance(phoneLoginValidator));

            container.Register(ServiceCriteria.For<ILoginService>().ImplementedBy<EmailLoginService>().Named("Email").Dependencies(new { loginValidator = container.Resolve<ILoginValidator>("EmailLoginValidator") }));
            container.Register(ServiceCriteria.For<ILoginService>().ImplementedBy<PhoneLoginService>().Named("Phone").Dependencies(new { loginValidator = container.Resolve<ILoginValidator>("PhoneLoginValidator") }));

            PhoneLoginService phoneLoginService1 = container.Resolve<PhoneLoginService>("Phone");
            PhoneLoginService phoneLoginService2 = container.Resolve<PhoneLoginService>("Phone");
            EmailLoginService emailLoginService = container.Resolve<EmailLoginService>("Email");

            Assert.IsNotNull(phoneLoginService1);
            Assert.IsNotNull(phoneLoginService2);
            Assert.IsNotNull(emailLoginService);

            string sessionKey1 = phoneLoginService1.Login("", "");
            string sessionKey2 = phoneLoginService2.Login("", "");
            string sessionKey3 = emailLoginService.Login("", "");

            Assert.IsFalse(String.IsNullOrEmpty(sessionKey1));
            Assert.IsFalse(String.IsNullOrEmpty(sessionKey2));
            Assert.IsFalse(String.IsNullOrEmpty(sessionKey3));

            Assert.AreNotEqual(phoneLoginService1.LoginValidator, phoneLoginValidator);
            Assert.AreNotEqual(phoneLoginService2.LoginValidator, phoneLoginValidator);
            Assert.AreNotEqual(emailLoginService.LoginValidator, emailLoginValidator);

            Assert.IsInstanceOfType(phoneLoginService1, typeof(PhoneLoginService));
            Assert.IsInstanceOfType(phoneLoginService2, typeof(PhoneLoginService));
            Assert.IsInstanceOfType(emailLoginService, typeof(EmailLoginService));

            Assert.IsTrue(typeof(ILoginService).IsAssignableFrom(phoneLoginService1.GetType()));
            Assert.IsTrue(typeof(ILoginService).IsAssignableFrom(phoneLoginService2.GetType()));
            Assert.IsTrue(typeof(ILoginService).IsAssignableFrom(emailLoginService.GetType()));
        }

        [TestMethod]
        public void TestMethod5()
        {
            UniIoC container = new UniIoC();

            ILoginValidator emailLoginValidator = new EmailLoginValidator();
            ILoginValidator phoneLoginValidator = new PhoneLoginValidator();

            container.Register(ServiceCriteria.For<ILoginValidator>().ImplementedBy<EmailLoginValidator>().Named("EmailLoginValidator").Instance(emailLoginValidator));
            container.Register(ServiceCriteria.For<ILoginValidator>().ImplementedBy<PhoneLoginValidator>().Named("PhoneLoginValidator").Instance(phoneLoginValidator));

            container.Register(ServiceCriteria.For<ILoginService>().ImplementedBy<EmailLoginService>().Named("Email").OnInstanceCreating(f => new EmailLoginService(f.Resolve<ILoginValidator>("EmailLoginValidator"))));
            container.Register(ServiceCriteria.For<ILoginService>().ImplementedBy<PhoneLoginService>().Named("Phone").OnInstanceCreating(f => new PhoneLoginService(f.Resolve<ILoginValidator>("PhoneLoginValidator"))));

            ILoginService phoneLoginService1 = container.Resolve<ILoginService>("Phone");
            ILoginService phoneLoginService2 = container.Resolve<ILoginService>("Phone");
            ILoginService emailLoginService = container.Resolve<ILoginService>("Email");

            Assert.IsNotNull(phoneLoginService1);
            Assert.IsNotNull(phoneLoginService2);
            Assert.IsNotNull(emailLoginService);

            string sessionKey1 = phoneLoginService1.Login("", "");
            string sessionKey2 = phoneLoginService2.Login("", "");
            string sessionKey3 = emailLoginService.Login("", "");

            Assert.IsFalse(String.IsNullOrEmpty(sessionKey1));
            Assert.IsFalse(String.IsNullOrEmpty(sessionKey2));
            Assert.IsFalse(String.IsNullOrEmpty(sessionKey3));

            Assert.AreNotEqual(phoneLoginService1.LoginValidator, phoneLoginValidator);
            Assert.AreNotEqual(phoneLoginService2.LoginValidator, phoneLoginValidator);
            Assert.AreNotEqual(emailLoginService.LoginValidator, emailLoginValidator);

            Assert.IsInstanceOfType(phoneLoginService1, typeof(PhoneLoginService));
            Assert.IsInstanceOfType(phoneLoginService2, typeof(PhoneLoginService));
            Assert.IsInstanceOfType(emailLoginService, typeof(EmailLoginService));

            Assert.IsTrue(typeof(ILoginService).IsAssignableFrom(phoneLoginService1.GetType()));
            Assert.IsTrue(typeof(ILoginService).IsAssignableFrom(phoneLoginService2.GetType()));
            Assert.IsTrue(typeof(ILoginService).IsAssignableFrom(emailLoginService.GetType()));
        }

        [TestMethod]
        public void TestMethod6()
        {
            UniIoC container = new UniIoC();

            container.Register(ServiceCriteria.For<ILoginValidator>().ImplementedBy<EmailLoginValidator>().Named("EmailLoginValidator"));
            container.Register(ServiceCriteria.For<ILoginValidator>().ImplementedBy<PhoneLoginValidator>().Named("PhoneLoginValidator"));

            container.Register(ServiceCriteria.For<ILoginService>().ImplementedBy<EmailLoginService>().Named("Email").OnInstanceCreating(f => new EmailLoginService(f.Resolve<ILoginValidator>("EmailLoginValidator"))));
            container.Register(ServiceCriteria.For<ILoginService>().ImplementedBy<PhoneLoginService>().Named("Phone").OnInstanceCreating(f => new PhoneLoginService(f.Resolve<ILoginValidator>("PhoneLoginValidator"))));

            ILoginService phoneLoginService1 = container.Resolve<ILoginService>("Phone");
            ILoginService phoneLoginService2 = container.Resolve<ILoginService>("Phone");
            ILoginService emailLoginService = container.Resolve<ILoginService>("Email");

            Assert.IsNotNull(phoneLoginService1);
            Assert.IsNotNull(phoneLoginService2);
            Assert.IsNotNull(emailLoginService);

            string sessionKey1 = phoneLoginService1.Login("", "");
            string sessionKey2 = phoneLoginService2.Login("", "");
            string sessionKey3 = emailLoginService.Login("", "");

            Assert.IsFalse(String.IsNullOrEmpty(sessionKey1));
            Assert.IsFalse(String.IsNullOrEmpty(sessionKey2));
            Assert.IsFalse(String.IsNullOrEmpty(sessionKey3));

            Assert.AreNotEqual(phoneLoginService1.LoginValidator, phoneLoginService2.LoginValidator);

            Assert.IsInstanceOfType(phoneLoginService1, typeof(PhoneLoginService));
            Assert.IsInstanceOfType(phoneLoginService2, typeof(PhoneLoginService));
            Assert.IsInstanceOfType(emailLoginService, typeof(EmailLoginService));

            Assert.IsTrue(typeof(ILoginService).IsAssignableFrom(phoneLoginService1.GetType()));
            Assert.IsTrue(typeof(ILoginService).IsAssignableFrom(phoneLoginService2.GetType()));
            Assert.IsTrue(typeof(ILoginService).IsAssignableFrom(emailLoginService.GetType()));
        }

        [TestMethod]
        public void TestMethod7()
        {
            UniIoC container = new UniIoC();

            container.Register(ServiceCriteria.For<ILoginValidator>().ImplementedBy<EmailLoginValidator>().Named("EmailLoginValidator").LifeCycle(LifeCycleEnum.Singleton));
            container.Register(ServiceCriteria.For<ILoginValidator>().ImplementedBy<PhoneLoginValidator>().Named("PhoneLoginValidator").LifeCycle(LifeCycleEnum.Singleton));

            container.Register(ServiceCriteria.For<ILoginService>().ImplementedBy<EmailLoginService>().Named("Email").OnInstanceCreating(f => new EmailLoginService(f.Resolve<ILoginValidator>("EmailLoginValidator"))));
            container.Register(ServiceCriteria.For<ILoginService>().ImplementedBy<PhoneLoginService>().Named("Phone").OnInstanceCreating(f => new PhoneLoginService(f.Resolve<ILoginValidator>("PhoneLoginValidator"))));

            ILoginService phoneLoginService1 = container.Resolve<ILoginService>("Phone");
            ILoginService phoneLoginService2 = container.Resolve<ILoginService>("Phone");
            ILoginService emailLoginService = container.Resolve<ILoginService>("Email");

            Assert.IsNotNull(phoneLoginService1);
            Assert.IsNotNull(phoneLoginService2);
            Assert.IsNotNull(emailLoginService);

            string sessionKey1 = phoneLoginService1.Login("", "");
            string sessionKey2 = phoneLoginService2.Login("", "");
            string sessionKey3 = emailLoginService.Login("", "");

            Assert.IsFalse(String.IsNullOrEmpty(sessionKey1));
            Assert.IsFalse(String.IsNullOrEmpty(sessionKey2));
            Assert.IsFalse(String.IsNullOrEmpty(sessionKey3));

            Assert.AreEqual(phoneLoginService1.LoginValidator, phoneLoginService2.LoginValidator);

            Assert.IsInstanceOfType(phoneLoginService1, typeof(PhoneLoginService));
            Assert.IsInstanceOfType(phoneLoginService2, typeof(PhoneLoginService));
            Assert.IsInstanceOfType(emailLoginService, typeof(EmailLoginService));

            Assert.IsTrue(typeof(ILoginService).IsAssignableFrom(phoneLoginService1.GetType()));
            Assert.IsTrue(typeof(ILoginService).IsAssignableFrom(phoneLoginService2.GetType()));
            Assert.IsTrue(typeof(ILoginService).IsAssignableFrom(emailLoginService.GetType()));
        }

        [TestMethod]
        public void TestMethod8()
        {
            UniIoC container = new UniIoC();

            container.Register(ServiceCriteria.For<ILoginValidator>().ImplementedBy<EmailLoginValidator>().Named("EmailLoginValidator").LifeCycle(LifeCycleEnum.Singleton));
            container.Register(ServiceCriteria.For<ILoginValidator>().ImplementedBy<PhoneLoginValidator>().Named("PhoneLoginValidator").LifeCycle(LifeCycleEnum.Singleton));

            container.Register(ServiceCriteria.For<ILoginService>().ImplementedBy<EmailLoginService>().Named("Email").Dependencies(new { loginValidator = new EmailLoginValidator() }).OnIntercepting(f => f.Proceed()));
            container.Register(ServiceCriteria.For<ILoginService>().ImplementedBy<PhoneLoginService>().Named("Phone").OnInstanceCreating(f => new PhoneLoginService(f.Resolve<ILoginValidator>("PhoneLoginValidator"))));

            ILoginService phoneLoginService1 = container.Resolve<ILoginService>("Phone");
            ILoginService phoneLoginService2 = container.Resolve<ILoginService>("Phone");
            ILoginService emailLoginService = container.Resolve<ILoginService>("Email");

            Assert.IsNotNull(phoneLoginService1);
            Assert.IsNotNull(phoneLoginService2);
            Assert.IsNotNull(emailLoginService);

            string sessionKey1 = phoneLoginService1.Login("", "");
            string sessionKey2 = phoneLoginService2.Login("", "");
            string sessionKey3 = emailLoginService.Login("", "");

            Assert.IsFalse(String.IsNullOrEmpty(sessionKey1));
            Assert.IsFalse(String.IsNullOrEmpty(sessionKey2));
            Assert.IsFalse(String.IsNullOrEmpty(sessionKey3));

            Assert.AreEqual(phoneLoginService1.LoginValidator, phoneLoginService2.LoginValidator);

            Assert.IsInstanceOfType(phoneLoginService1, typeof(PhoneLoginService));
            Assert.IsInstanceOfType(phoneLoginService2, typeof(PhoneLoginService));
            Assert.IsNotInstanceOfType(emailLoginService, typeof(EmailLoginService));

            Assert.IsTrue(typeof(ILoginService).IsAssignableFrom(phoneLoginService1.GetType()));
            Assert.IsTrue(typeof(ILoginService).IsAssignableFrom(phoneLoginService2.GetType()));
            Assert.IsTrue(typeof(ILoginService).IsAssignableFrom(emailLoginService.GetType()));
        }

        [TestMethod]
        public void TestMethod9()
        {
            UniIoC container = new UniIoC();

            container.Register(ServiceCriteria.For<ILoginValidator>().ImplementedBy<EmailLoginValidator>().Named("EmailLoginValidator").LifeCycle(LifeCycleEnum.Singleton));
            container.Register(ServiceCriteria.For<ILoginValidator>().ImplementedBy<PhoneLoginValidator>().Named("PhoneLoginValidator").LifeCycle(LifeCycleEnum.Singleton));

            container.Register(ServiceCriteria.For<ILoginService>().ImplementedBy<EmailLoginService>().Named("Email").Dependencies(new { loginValidator = new EmailLoginValidator() }).OnIntercepting(OnIntercepting));
            container.Register(ServiceCriteria.For<ILoginService>().ImplementedBy<PhoneLoginService>().Named("Phone").OnInstanceCreating(f => new PhoneLoginService(f.Resolve<ILoginValidator>("PhoneLoginValidator"))));

            ILoginService phoneLoginService1 = container.Resolve<ILoginService>("Phone");
            ILoginService phoneLoginService2 = container.Resolve<ILoginService>("Phone");
            ILoginService emailLoginService = container.Resolve<ILoginService>("Email");

            Assert.IsNotNull(phoneLoginService1);
            Assert.IsNotNull(phoneLoginService2);
            Assert.IsNotNull(emailLoginService);

            string sessionKey1 = phoneLoginService1.Login("", "");
            string sessionKey2 = phoneLoginService2.Login("", "");
            string sessionKey3 = emailLoginService.Login("", "");

            Assert.IsFalse(String.IsNullOrEmpty(sessionKey1));
            Assert.IsFalse(String.IsNullOrEmpty(sessionKey2));
            Assert.IsFalse(String.IsNullOrEmpty(sessionKey3));

            Assert.AreEqual(phoneLoginService1.LoginValidator, phoneLoginService2.LoginValidator);

            Assert.IsInstanceOfType(phoneLoginService1, typeof(PhoneLoginService));
            Assert.IsInstanceOfType(phoneLoginService2, typeof(PhoneLoginService));
            Assert.IsNotInstanceOfType(emailLoginService, typeof(EmailLoginService));

            Assert.IsTrue(typeof(ILoginService).IsAssignableFrom(phoneLoginService1.GetType()));
            Assert.IsTrue(typeof(ILoginService).IsAssignableFrom(phoneLoginService2.GetType()));
            Assert.IsTrue(typeof(ILoginService).IsAssignableFrom(emailLoginService.GetType()));
        }

        public void OnIntercepting(IInvocation invocation)
        {
            Console.WriteLine("{0} method is invoked.", invocation.Method.Name);
            invocation.Proceed();
        }

        [TestMethod]
        public void TestMethod10()
        {
            //Eğer interceptor kullanılacaksa For<> methodu ile oluşturulacak class'ın interface'i belirtilmeli. Böylece arka tarafta oluşan proxy class interface'i implemente ettiği için resolve olduğunda tekrar interface dönüşümünde sıkıntı olmayacaktır. Eğer interfacesiz kullanılırsa proxy class For<> ile verilen concrete type'dan inherit olmadığı için resolve ederken concrete class'a convert olamadığı için runtime'da hata verilir.
            UniIoC container = new UniIoC();

            container.Register(ServiceCriteria.For<ILoginValidator>().ImplementedBy<EmailLoginValidator>().Named("EmailLoginValidator").LifeCycle(LifeCycleEnum.Singleton));
            container.Register(ServiceCriteria.For<ILoginValidator>().ImplementedBy<PhoneLoginValidator>().Named("PhoneLoginValidator").LifeCycle(LifeCycleEnum.Singleton));

            container.Register(ServiceCriteria.For<ILoginService>().ImplementedBy<EmailLoginService>().Named("Email").OnInstanceCreating(f => new EmailLoginService(new EmailLoginValidator())).OnIntercepting(OnIntercepting));
            container.Register(ServiceCriteria.For<ILoginService>().ImplementedBy<PhoneLoginService>().Named("Phone").OnInstanceCreating(f => new PhoneLoginService(f.Resolve<ILoginValidator>("PhoneLoginValidator"))));

            var phoneLoginService1 = container.Resolve<ILoginService>("Phone");
            var phoneLoginService2 = container.Resolve<ILoginService>("Phone");
            var emailLoginService = container.Resolve<ILoginService>("Email");

            Assert.IsNotNull(phoneLoginService1);
            Assert.IsNotNull(phoneLoginService2);
            Assert.IsNotNull(emailLoginService);

            string sessionKey1 = phoneLoginService1.Login("", "");
            string sessionKey2 = phoneLoginService2.Login("", "");
            string sessionKey3 = emailLoginService.Login("", "");

            Assert.IsFalse(String.IsNullOrEmpty(sessionKey1));
            Assert.IsFalse(String.IsNullOrEmpty(sessionKey2));
            Assert.IsFalse(String.IsNullOrEmpty(sessionKey3));

            Assert.AreEqual(phoneLoginService1.LoginValidator, phoneLoginService2.LoginValidator);

            Assert.IsInstanceOfType(phoneLoginService1, typeof(PhoneLoginService));
            Assert.IsInstanceOfType(phoneLoginService2, typeof(PhoneLoginService));
            Assert.IsNotInstanceOfType(emailLoginService, typeof(EmailLoginService));

            Assert.IsTrue(typeof(ILoginService).IsAssignableFrom(phoneLoginService1.GetType()));
            Assert.IsTrue(typeof(ILoginService).IsAssignableFrom(phoneLoginService2.GetType()));
            Assert.IsTrue(typeof(ILoginService).IsAssignableFrom(emailLoginService.GetType()));
        }

        [TestMethod]
        public void TestMethod11()
        {
            UniIoC container = new UniIoC();

            container.Register(ServiceCriteria.For<IShape>().ImplementedBy<Circle>());

            IShape circle = container.Resolve<IShape>();

            Assert.IsNotNull(circle);

            Assert.IsInstanceOfType(circle, typeof(Circle));

            Assert.IsTrue(typeof(IShape).IsAssignableFrom(circle.GetType()));
        }

        [TestMethod]
        public void TestMethod12()
        {
            UniIoC container = new UniIoC();

            container.Register(ServiceCriteria.For<IShape>().ImplementedBy<Circle>().Named("Circle"));
            container.Register(ServiceCriteria.For<IShape>().ImplementedBy<Square>().Named("Square"));

            IShape circle = container.Resolve<IShape>("Circle");
            IShape square = container.Resolve<IShape>("Square");

            Assert.IsNotNull(circle);
            Assert.IsNotNull(square);

            Assert.IsInstanceOfType(circle, typeof(Circle));
            Assert.IsInstanceOfType(square, typeof(Square));

            Assert.IsTrue(typeof(IShape).IsAssignableFrom(circle.GetType()));
            Assert.IsTrue(typeof(IShape).IsAssignableFrom(square.GetType()));
        }

        [TestMethod]
        public void TestMethod13()
        {
            UniIoC container = new UniIoC();

            container.Register(ServiceCriteria.For<NewInterceptor>());

            container.Register(ServiceCriteria.For<ILoginValidator>().ImplementedBy<EmailLoginValidator>().Named("EmailLoginValidator").LifeCycle(LifeCycleEnum.Singleton));

            container.Register(ServiceCriteria.For<ILoginService>().ImplementedBy<EmailLoginService>().Named("Email").OnInstanceCreating(f => new EmailLoginService(f.Resolve<ILoginValidator>("EmailLoginValidator"))).Interceptor<NewInterceptor>());



            ILoginService emailLoginService = container.Resolve<ILoginService>("Email");

            Assert.IsNotNull(emailLoginService);

            Assert.IsFalse(String.IsNullOrEmpty(emailLoginService.Login("hede", "hode")));
        }

        [TestMethod]
        public void TestMethod14()
        {
            ILoginService loginServiceProxy = new LoginServiceProxy(new NewInterceptor(), new PhoneLoginService(new PhoneLoginValidator()));

            string sessionKey = loginServiceProxy.Login("", "");
        }

        [TestMethod]
        public void TestMethod15()
        {
            UniIoC container = new UniIoC();

            container.Register(ServiceCriteria.For<IOrm>().ImplementedBy<Orm>().Named("northwindSqlite").LifeCycle(LifeCycleEnum.Singleton).OnInstanceCreating(f => new Orm()).OnIntercepting(f => { Console.WriteLine(f.Method.Name); }));

            IOrm a1 = container.Resolve<IOrm>("northwindSqlite");

            var a2 = a1.Query<Orm>();
        }

        [TestMethod]
        public void TestMethod16()
        {
            UniIoC container = new UniIoC();

            container.Register(ServiceCriteria.For<Circle>());
            container.Register(ServiceCriteria.For<Square>());

            IShape circle = container.Resolve<Circle>();
            IShape square = container.Resolve<Square>();

            Assert.IsNotNull(circle);
            Assert.IsNotNull(square);

            Assert.IsInstanceOfType(circle, typeof(Circle));
            Assert.IsInstanceOfType(square, typeof(Square));

            Assert.IsTrue(typeof(IShape).IsAssignableFrom(circle.GetType()));
            Assert.IsTrue(typeof(IShape).IsAssignableFrom(square.GetType()));
        }

        [TestMethod]
        public void TestMethod17()
        {
            UniIoC container = new UniIoC();

            container.Register<IClient, SomeClient>();
            container.Register<IService, SomeService>();

            IClient client = container.Resolve<IClient>();

            Assert.IsNotNull(client);

            Assert.IsInstanceOfType(client, typeof(SomeClient));

            Assert.IsTrue(typeof(IClient).IsAssignableFrom(client.GetType()));
        }

        [TestMethod]
        public void TestMethod18()
        {
            UniIoC container = new UniIoC();

            container.Register<IClient, SomeClient>();
            container.Register<IService, SomeService>();
            container.Register<IService, SomeService>(f=>f.RegisterBehavior(RegisterBehaviorEnum.Keep));

            IClient client = container.Resolve<IClient>();

            Assert.IsNotNull(client);

            Assert.IsInstanceOfType(client, typeof(SomeClient));

            Assert.IsTrue(typeof(IClient).IsAssignableFrom(client.GetType()));

            container.UnRegister<IClient>();

            try
            {
                container.Resolve<IClient>();
            }
            catch (Exception ex)
            {

            }
        }
    }
}