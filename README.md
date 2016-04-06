# Uni.IoC

`Uni.IoC` is a simple, fast dependency injection library.

##How To Install It?
Install from `Nuget`, you should write Package Manager Console below code and `Uni.IoC` will be installed automatically.
```
Install-Package Uni.IoC
```
By the way, you can also reach `Uni.IoC` `NuGet` package from https://www.nuget.org/packages/Uni.IoC/ address.

##How Do You Use It?
It is easy to use. Let's have a look at a simple example.

Firstly, UniIoC object is created named container. And register a interface to a concrete type. Lastly, resolve instance according to registered interface.

```csharp
UniIoC container = new UniIoC();

container.Register(ServiceCriteria.For<IShape>().ImplementedBy<Circle>());

var shape = container.Resolve<IShape>();
```
