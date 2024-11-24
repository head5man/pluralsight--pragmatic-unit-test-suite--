### Styles of Unit Testing
* output verification (functional)
* state verification
* collaboration verification

### Hexagonal Architecture

```mermaid
flowchart 
third{{<br>3rd party<br>system<br><br>}}
mbus{{<br>Message<br>bus<br><br>}}
smtp{{<br>SMTP<br>service<br><br>}}
third<--uses-->port1<-->domain
domain<--uses-->port3<-->mbus
domain<--uses-->port2<-->smtp
subgraph Application Services
domain{{<br>Business logic<br>domain model<br><br>}}
port3
port2
port1
end
```

### What is Implementation Detail?

Addresses an immediate goal of the client code and addresses that goal completely.
```C#
public class User 
{
    public string Name { get; set; }
    // Normalize name is implementation detail 
    // and should be handled by the class itself
    public string NormalizeName(string name)
    {
        string result = (name ?? "").Trim();

        return (result.Length > 50)
            ? result.Substring(0, 50) : result;
    }
}

// ==>
public class User 
{
    private string _name;

    public string Name 
    { 
        get => _name;
        set => _name = NormalizeName(value);
    }

    private string NormalizeName(string name)
    {
        string result = (name ?? "").Trim();

        return (result.Length > 50)
            ? result.Substring(0, 50) : result;
    }
}

public class UserController 
{
    public void ChangeName(int userId, string newName)
    {
        User user = GetUserFromDb(userId);
        User.Name = newName;
    }
}
```

```C#
void GetProduct(int id)
{
    Order order = GetOrderFromDB(id);
    // ! exposing implementation detail
    Product product = order.Products.SingleOrDefault(x => x.PartNumber == "M0312");
    return product;
}

void GetProduct(int id)
{
    Order order = GetOrderFromDB(id);
    // keeping details encapsulated
    Product product = order.GetProduct("M0312");
    return product;
}
```

### Functional
Test are carried out providing input value and verifying output.

Best protection against false positives.

Easy to maintain.

Participants:

```C#
// Immutable - input variable - 
double Calculate(double x, double y)
{
    return x * x + y * y;
}
```
```c#
// Mutable - Collaborator -
double Calculate(ICalculator calculator, double x, double y)
{
    calculator.Push(x);
    calculator.Push(y);
    return calculator.CalculateFormula();
}
```

### State verification
Has input or instigator and SUT is expected to change state of an object that is part of the public API.

```mermaid
flowchart
subgraph SUT
0(( ));11(( ));12(( ));21(( ));22(( ))
31(( ));32(( ));33(( ))
style 11 fill:blue
style 12 fill:blue
style 0 fill:none
style 21 fill:orange
style 22 fill:orange
0-->11-->12
0-->21-->22
end
22 & 12--/-->31
22-->32
12-->33
```

### Collaboration Verification
Can be internal or external. In order to be part of the public API it has to be called by an outside layer.

Internal communication (inside the hexagon) is usually implementation detail and verifying against it is more likely to produce false positives and maintainablity is poor.

As a guideline you should verify collaborations at the very edges of your system.

![](internal-collaboration.png)
We can see in the diagram that both of the classes `Order` and `User` transition to a new state and can be observed and verified.
![](internal-collaboration-hexagon.png)
External communication (outside the hexagon) is part of communication state.

Wrap external services with your own gateways.
Commision the communication with other hexagons - to application services.

![](external-collaboration-hexagon.png)
![](external-collaboration.png)

### Black-box vs. White-box Testing

Black-box testing is testing without knowing the internal structure.

White-box testing is testing the internal structure.