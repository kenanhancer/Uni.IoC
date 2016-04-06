# Uni.IoC

`Uni.IoC` is a simple and fast dependency injection library.

##How To Install It?
Install from `Nuget`, you should write Package Manager Console below code and `Uni.IoC` will be installed automatically.
```
Install-Package Uni.IoC
```
By the way, you can also reach `Uni.IoC` `NuGet` package from https://www.nuget.org/packages/Uni.IoC/ address.

##How Do You Use It?
It is easy to use. Let's have a look at a simple example.

Firstly, UniIoC object is created named container. And register a interface to a concrete type. Lastly, resolve instance according to registered interface as shown below code.

```csharp
UniIoC container = new UniIoC();

container.Register(ServiceCriteria.For<IShape>().ImplementedBy<Circle>());

var shape = container.Resolve<IShape>();
```

##Resolving different concrete types which implement same interface
If you have different concrete types which imlement same interface, you can register them with different names. As you can see below sample code, there is one `IShape` interface and two concrete types `Circle`, `Square` which use that interface.


```csharp
public interface IShape
{
}

public class Circle : IShape
{
}

public class Square : IShape
{
}
```

```csharp
UniIoC container = new UniIoC();

container.Register(ServiceCriteria.For<IShape>().ImplementedBy<Circle>().Named("Circle"));
container.Register(ServiceCriteria.For<IShape>().ImplementedBy<Square>().Named("Square"));

var circle = container.Resolve<IShape>("Circle");
var square = container.Resolve<IShape>("Square");
```
