# Covariance, Contravariance, and Invariance in .NET

> A complete, ground-up explanation built for senior engineers who want the *intuition*, not just the syntax.

---

## Table of Contents

1. [The One Idea Behind Everything](#part-1-the-one-idea-behind-everything)
2. [Liskov Substitution Principle (LSP)](#part-2-liskov-substitution-principle-lsp)
3. [Now — Generics](#part-3-now--generics)
4. [The Trick That Makes Generics Click](#part-4-the-trick-that-makes-generics-click)
5. [Real-World Example: Trade Event Handlers](#part-5-real-world-example-trade-event-handlers)
6. [Real-World Example: Repository Pattern](#part-6-real-world-example-repository-pattern)
7. [Real-World Example: Message Bus / RabbitMQ Handlers](#part-7-real-world-example-message-bus--rabbitmq-handlers)
8. [The Final, Simple Picture](#part-8-the-final-simple-picture)
9. [Built-in .NET Variant Types Reference](#part-9-built-in-net-variant-types-reference)

---

## Part 1: The One Idea Behind Everything

Before we touch any keyword, you need to understand **one single idea**. Everything else is just this idea repeated in different shapes.

> **The Idea:** When you say *"give me an X,"* someone can give you an X — or anything that **behaves like an X or better**.

That's it. That's the whole topic. This is called **substitutability**.

### A real-world example (enterprise software)

Imagine your team contract says: *"We will provide an `IPaymentProcessor`."*

You deliver a `StripePaymentProcessor`. ✅ Fine — it **is** an `IPaymentProcessor`.

Now reverse it. Imagine the contract says: *"We will provide a `StripePaymentProcessor` specifically."*

You deliver "some `IPaymentProcessor`" — could be PayPal, could be a mock, could be anything. ❌ Not fine. The contract asked for something specific.

**Substitution rule:** You can replace something with a **more specific** version when receiving, but not with a **more general** version when receiving.

Hold onto this. We will use it the entire way through.

---

## Part 2: Liskov Substitution Principle (LSP)

Barbara Liskov took this exact common-sense rule and turned it into a software principle.

### Her rule, in plain words

> If your code expects a `User`, and you hand it an `AdminUser`, the code should still work correctly — because an `AdminUser` **is** a `User`.

That's LSP. Nothing more.

### Now apply it to method signatures

A method has two parts:
- **Parameters** (what goes IN)
- **Return type** (what comes OUT)

When you override a method in a child class, LSP asks: *"Can the child's method still be used wherever the parent's method was expected?"*

To answer that, we look at IN and OUT separately.

---

### LSP Rule for Return Types (OUT direction)

Parent class:

```csharp
class UserRepository
{
    public virtual User GetById(int id) => new User();
}
```

Caller code:

```csharp
User user = repository.GetById(42);  // I expect a User
```

Now a child class overrides this method. **What is the child allowed to return?**

**Option A — child returns an `AdminUser`:**

```csharp
class AdminUserRepository : UserRepository
{
    public override AdminUser GetById(int id) => new AdminUser();   // returns more specific
}
```

Caller does `User user = repository.GetById(42);` — gets an AdminUser. **An AdminUser is a User. Works.** ✅

**Option B — child returns an `object` (more general):**

```csharp
public override object GetById(int id) => "hello";   // returns more general
```

Caller does `User user = repository.GetById(42);` — gets a string. **Crash.** ❌

#### The rule for return types:

> The child can return the **same type or a more specific type**. Never more general.
>
> This is called **covariant return types**.

**"Covariant"** simply means: *"varies in the same direction as the inheritance."*
Inheritance goes `User → AdminUser` (more specific). Returns can also go `User → AdminUser` (more specific). Same direction. **Co**-variant.

---

### LSP Rule for Parameters (IN direction)

Parent class:

```csharp
class NotificationService
{
    public virtual void Send(EmailMessage email) { ... }
}
```

Caller code:

```csharp
service.Send(new EmailMessage());   // I'm passing an EmailMessage
```

Now a child class overrides this method. **What parameter type is it allowed to accept?**

**Option A — child accepts `Message` (more general):**

```csharp
public override void Send(Message message) { ... }   // accepts more general
```

Caller passes an EmailMessage. **An EmailMessage is a Message. Works.** ✅ The method can handle it because Message is broader.

**Option B — child accepts `MarketingEmailMessage` (more specific):**

```csharp
public override void Send(MarketingEmailMessage email) { ... }   // accepts more specific
```

Caller passes a regular EmailMessage (not marketing). **Crash.** ❌ The method only handles marketing emails.

#### The rule for parameters:

> The child can accept the **same type or a more general type**. Never more specific.
>
> This is called **contravariant parameter types**.

**"Contravariant"** means: *"varies in the OPPOSITE direction of the inheritance."*
Inheritance goes `Message → EmailMessage` (more specific). But parameters can go `EmailMessage → Message` (more general). Opposite direction. **Contra**-variant.

> **Note:** C# doesn't actually let you change parameter types when overriding — but the *theoretical rule* still tells us which direction is safe. We'll use this exact rule for generics next.

---

### Lock This In Before Moving On

Two rules. Memorize them:

| Position | Safe direction | Name |
|---|---|---|
| **Return type** (OUT) | More specific is OK (`User → AdminUser`) | **Covariant** |
| **Parameter** (IN) | More general is OK (`EmailMessage → Message`) | **Contravariant** |

These rules exist because of one simple thing: **substitutability must not break the caller.**

Stop here. Read this section twice. Move on only when this feels obvious.

---

## Part 3: Now — Generics

Generics make this idea apply at a bigger level. Instead of asking "*can an AdminUser method replace a User method?*" we ask:

> **Can `IRepository<AdminUser>` replace `IRepository<User>` — or vice versa?**

The answer depends on **what's inside the interface** — specifically, whether `T` is used for OUT, IN, or BOTH.

---

### Case 1: T is used only for OUT (returning) → `out T` → Covariant

Imagine an interface that **only gives you things**, never takes them in:

```csharp
public interface IReadOnlyRepository<out T>
{
    T GetById(int id);   // T comes OUT
    IEnumerable<T> GetAll();  // T comes OUT
}
```

You can read T out, that's it. No way to put T in.

#### Why is this safe to substitute?

```csharp
IReadOnlyRepository<AdminUser> adminRepo = new AdminUserRepository();
IReadOnlyRepository<User>      userRepo  = adminRepo;   // ✅ legal
```

Why does this work? Because:
- `userRepo.GetById(1)` returns... a User (according to its type).
- Underneath, it's actually returning an AdminUser.
- **An AdminUser is a User.** No one gets surprised. No crash.

This is the *same* rule from Part 2 — covariant return types — just applied to the generic parameter `T`.

That's why the keyword is **`out`**: T appears in OUT positions only.

> **Memorize:** `out T` = T flows out → covariance → can substitute `IReadOnlyRepository<AdminUser>` for `IReadOnlyRepository<User>`.

---

### Case 2: T is used only for IN (consuming) → `in T` → Contravariant

Now imagine an interface that **only takes things in**, never gives them out:

```csharp
public interface IValidator<in T>
{
    bool Validate(T item);   // T goes IN
}
```

You can put T in, that's it. No way to read T out.

#### Why is this safe to substitute — but the OPPOSITE direction?

```csharp
IValidator<User>      userValidator  = new UserValidator();
IValidator<AdminUser> adminValidator = userValidator;   // ✅ legal — looks weird but it's safe
```

Wait. Why is this OK? Let me make it intuitive.

`userValidator` says: *"Give me any User, I'll validate it."* That's a **broad promise** — it can validate AdminUsers, GuestUsers, anything that's a User.

`adminValidator` is supposed to handle: *"Give me an AdminUser, I'll validate it."*

Can the user-validator fulfill the admin-validator's job? **Yes.** If you ask it to validate an AdminUser, no problem — an AdminUser is a User, and the user-validator validates any User.

So we're substituting *a more capable consumer* for *a less capable one*. Totally safe.

This is the *same* rule from Part 2 — contravariant parameters — applied to the generic parameter `T`.

That's why the keyword is **`in`**: T appears in IN positions only.

#### And why the reverse is FORBIDDEN

```csharp
IValidator<AdminUser> adminValidator = new AdminOnlyValidator();   // only validates Admins
IValidator<User>      userValidator  = adminValidator;             // ❌ ILLEGAL
```

If this were allowed, then somewhere later:

```csharp
userValidator.Validate(new GuestUser());   // userValidator promises to validate any User
```

But underneath, it's actually an `AdminOnlyValidator`. It receives a GuestUser and... **crash.** It doesn't know how to validate guest users — perhaps it tries to access `admin.PrivilegeLevel` which doesn't exist on `GuestUser`.

The compiler refuses to let this assignment happen. That's contravariance protecting you.

> **Memorize:** `in T` = T flows in → contravariance → can substitute `IValidator<User>` for `IValidator<AdminUser>` (broader replaces narrower).

---

### Case 3: T is used for BOTH → no keyword → Invariant

```csharp
public interface IRepository<T>
{
    T GetById(int id);     // T goes OUT
    void Add(T item);      // T comes IN
}
```

Now `T` is used in both directions. Can we substitute `IRepository<AdminUser>` for `IRepository<User>`? Let's check:

If we allowed:

```csharp
IRepository<AdminUser> adminRepo = new AdminUserRepository();
IRepository<User>      userRepo  = adminRepo;   // pretend this compiled
userRepo.Add(new GuestUser());                  // looks fine — GuestUser is a User
AdminUser admin = adminRepo.GetById(1);         // 💥 it's a GuestUser, not an AdminUser
```

**Broken.** So this direction is forbidden.

What about the other direction?

```csharp
IRepository<User>      userRepo  = new UserRepository();
IRepository<AdminUser> adminRepo = userRepo;    // pretend this compiled
AdminUser admin = adminRepo.GetById(1);         // 💥 might return a regular User
```

**Also broken.**

Conclusion: when T is used both ways, **no substitution is safe**. The type is **invariant**. No keyword. This is the C# default.

This is why `List<T>` — which has both `Add(T)` and `T this[int]` — is invariant. `List<AdminUser>` and `List<User>` are completely unrelated types as far as the compiler is concerned.

---

## Part 4: The Trick That Makes Generics Click

Here's the part most people miss:

> When we talk about variance with generics, the **substitution rule applies to the WHOLE GENERIC TYPE**, not to the inner T.

Read that line again.

For LSP (Part 2), we substituted `AdminUser` for `User`.
For generics (Part 3), we substitute `IReadOnlyRepository<AdminUser>` for `IReadOnlyRepository<User>` — the **whole thing**, brackets and all, is what's being substituted.

The `in` / `out` keyword on the inner `T` is just the compiler's way of telling you:
- **`out T`** → the whole `IFoo<T>` follows the same direction as T → covariant.
- **`in T`** → the whole `IFoo<T>` follows the OPPOSITE direction from T → contravariant.
- **No keyword** → no substitution allowed → invariant.

So your Liskov instinct was correct all along. The only adjustment is: apply the rule to **`IFoo<AdminUser>` vs `IFoo<User>`**, not to **AdminUser vs User**.

---

## Part 5: Real-World Example — Trade Event Handlers

Now let's apply everything to a real enterprise scenario you might encounter in a financial system.

```csharp
// Domain: a base trade event and several specialized variants
public abstract class TradeEvent
{
    public string TradeId { get; init; }
    public DateTime Timestamp { get; init; }
}

public class RiskyTradeEvent : TradeEvent
{
    public decimal RiskScore { get; init; }
    public bool IsHighFrequency { get; init; }
}

public class BondTradeEvent : TradeEvent
{
    public decimal CouponRate { get; init; }
}
```

The handler interface:

```csharp
public interface ITradeHandler<in T> where T : TradeEvent
{
    void HandleTrade(T tradeEvent);   // T is INPUT only → in T → contravariant
}
```

`T` is used only as a parameter (IN). So this is **contravariant**. The `in` keyword is correct.

### What this means in practice

The handler **consumes** trade events. The "broader / more capable" handler is the one that accepts the **base** type `TradeEvent` — because it can handle anything. The "narrower / less capable" handler is the one that only handles `RiskyTradeEvent`.

The safe substitution is: **broader replaces narrower.**

```csharp
// A handler that accepts ANY TradeEvent (e.g., a logger or auditor)
public class AuditLogHandler : ITradeHandler<TradeEvent>
{
    public void HandleTrade(TradeEvent t)
        => Console.WriteLine($"[AUDIT] {t.TradeId} at {t.Timestamp}");
}

ITradeHandler<TradeEvent>      generalAudit = new AuditLogHandler();
ITradeHandler<RiskyTradeEvent> riskyAudit   = generalAudit;   // ✅ broader replaces narrower

riskyAudit.HandleTrade(new RiskyTradeEvent { TradeId = "T-1" });
// Works — AuditLogHandler accepts ANY TradeEvent, including RiskyTradeEvent
```

This is incredibly powerful in practice: **one cross-cutting handler** (logging, auditing, metrics) can be reused **everywhere** a specific handler is expected.

### Why the reverse is forbidden

```csharp
public class RiskyOnlyHandler : ITradeHandler<RiskyTradeEvent>
{
    public void HandleTrade(RiskyTradeEvent t)
    {
        // This code RELIES on RiskyTradeEvent-specific properties
        if (t.RiskScore > 0.8m)
            AlertRiskTeam(t);

        if (t.IsHighFrequency)
            ThrottleConnection(t);
    }
}

ITradeHandler<RiskyTradeEvent> riskyOnly = new RiskyOnlyHandler();
ITradeHandler<TradeEvent>      anyEvent  = riskyOnly;   // ❌ FORBIDDEN by compiler
```

If the compiler allowed that last line, then later somebody could do:

```csharp
anyEvent.HandleTrade(new BondTradeEvent { TradeId = "B-99" });
// This is a BondTradeEvent — no RiskScore, no IsHighFrequency
```

Which lands inside `RiskyOnlyHandler.HandleTrade`, which tries to access `.RiskScore` on a `BondTradeEvent`. **The property doesn't exist there. Compile-time crash if cast, runtime crash if reflected.**

So the compiler blocks the dangerous direction. That's contravariance keeping you safe.

---

## Part 6: Real-World Example — Repository Pattern

This is where covariance shines in everyday .NET architecture.

```csharp
public abstract class Entity
{
    public int Id { get; init; }
}

public class Customer : Entity
{
    public string Email { get; init; }
}

public class PremiumCustomer : Customer
{
    public decimal CreditLimit { get; init; }
}
```

A read-only repository (covariant — T only flows OUT):

```csharp
public interface IReadOnlyRepository<out T> where T : Entity
{
    T GetById(int id);
    IEnumerable<T> GetAll();
}
```

A full repository (invariant — T flows both ways):

```csharp
public interface IRepository<T> where T : Entity
{
    T GetById(int id);          // OUT
    void Add(T entity);         // IN
    void Update(T entity);      // IN
}
```

### Practical use of covariance

```csharp
public class PremiumCustomerRepository : IReadOnlyRepository<PremiumCustomer>
{
    public PremiumCustomer GetById(int id) => /* ... */;
    public IEnumerable<PremiumCustomer> GetAll() => /* ... */;
}

// In a service that only needs to read customers:
IReadOnlyRepository<PremiumCustomer> premiumRepo = new PremiumCustomerRepository();
IReadOnlyRepository<Customer>        customerRepo = premiumRepo;   // ✅ covariance

// Now any code expecting customers can use the premium repo:
foreach (Customer c in customerRepo.GetAll())
    Console.WriteLine(c.Email);
```

### Why this matters in real architectures

In Clean Architecture, returning `IReadOnlyRepository<T>` (covariant) instead of `IRepository<T>` (invariant) gives you:

1. **Polymorphic reads** — a `PremiumCustomerRepository` can be passed wherever a `Customer` reader is expected.
2. **Encapsulation** — callers can't accidentally mutate state through a read-only interface.
3. **Loose coupling** — services depending on `IReadOnlyRepository<Customer>` don't need to know about specific subtypes.

You'll notice this pattern in EF Core (`IQueryable<out T>`), MediatR, and most enterprise codebases.

---

## Part 7: Real-World Example — Message Bus / RabbitMQ Handlers

Let's say you're building a message dispatcher (RabbitMQ, Azure Service Bus, MediatR — same pattern):

```csharp
public interface IMessage
{
    Guid MessageId { get; }
    DateTime PublishedAt { get; }
}

public class OrderPlaced : IMessage { /* ... */ }
public class PaymentReceived : IMessage { /* ... */ }
public class InventoryReserved : IMessage { /* ... */ }
```

The handler interface — contravariant because handlers **consume** messages:

```csharp
public interface IMessageHandler<in TMessage> where TMessage : IMessage
{
    Task HandleAsync(TMessage message, CancellationToken ct);
}
```

### Cross-cutting handlers powered by contravariance

```csharp
// A universal logger that handles ANY message type
public class LoggingHandler : IMessageHandler<IMessage>
{
    public Task HandleAsync(IMessage msg, CancellationToken ct)
    {
        Console.WriteLine($"[LOG] {msg.GetType().Name} at {msg.PublishedAt}");
        return Task.CompletedTask;
    }
}

// The same instance can be used wherever a SPECIFIC handler is expected:
IMessageHandler<OrderPlaced>        orderLogger     = new LoggingHandler();   // ✅
IMessageHandler<PaymentReceived>    paymentLogger   = new LoggingHandler();   // ✅
IMessageHandler<InventoryReserved>  inventoryLogger = new LoggingHandler();   // ✅
```

**One implementation. Reused for every message type.** That's the production payoff of `in T`.

### What would NOT work (and why the compiler protects you)

```csharp
// A specialist handler that only knows about payments
public class PaymentSpecificHandler : IMessageHandler<PaymentReceived>
{
    public Task HandleAsync(PaymentReceived msg, CancellationToken ct)
    {
        ProcessRefund(msg.Amount);   // Amount only exists on PaymentReceived
        return Task.CompletedTask;
    }
}

IMessageHandler<PaymentReceived> paymentHandler = new PaymentSpecificHandler();
IMessageHandler<IMessage>        anyHandler     = paymentHandler;   // ❌ COMPILE ERROR
```

The compiler stops you because `anyHandler` would promise to handle **any** `IMessage`, but `PaymentSpecificHandler` only knows about `PaymentReceived`. Passing it an `OrderPlaced` would crash.

---

## Part 8: The Final, Simple Picture

Three rules. That's the whole topic.

| Keyword | T is used | Direction allowed | Name |
|---|---|---|---|
| `out T` | OUT only (returns) | `IFoo<Derived>` → `IFoo<Base>` | Covariance |
| `in T` | IN only (parameters) | `IFoo<Base>` → `IFoo<Derived>` | Contravariance |
| (none) | BOTH | No substitution | Invariance |

And the one unifying sentence:

> **You can always substitute a type that makes a stronger or equal promise. You can never substitute one that makes a weaker promise.**

- An `AdminUser` is a stronger promise than a `User` → can substitute as a return value.
- A `User`-validator is a stronger promise than an `AdminUser`-validator (because it handles more cases) → can substitute as a consumer.

That's all there is. Everything else — `out`, `in`, covariance, contravariance, invariance, LSP — is just this one idea applied in different positions.

---

## Part 9: Built-in .NET Variant Types Reference

For day-to-day reference, here are the variant generic types you'll actually use in .NET:

### Covariant (`out T`)
- `IEnumerable<out T>` — LINQ pipelines
- `IReadOnlyCollection<out T>`, `IReadOnlyList<out T>` — immutable views
- `IQueryable<out T>` — EF Core queries
- `Func<..., out TResult>` — return type of delegates
- `Task<out TResult>` (in some signatures) — async results

### Contravariant (`in T`)
- `IComparer<in T>` — sorting
- `IEqualityComparer<in T>` — equality checks
- `Action<in T>` — parameter types
- `Predicate<in T>` — filters
- `Func<in T1, in T2, ..., out TResult>` — parameters are contravariant, return is covariant

### Invariant (no keyword — most common)
- `List<T>`, `Dictionary<TKey, TValue>`, `HashSet<T>`
- `ICollection<T>`, `IList<T>`, `IDictionary<TKey, TValue>`
- Almost every concrete generic class

---

## Variance Rules — Cheat Sheet

```
┌─────────────────────────────────────────────────────────────┐
│  IF T appears only in RETURN positions (output)             │
│     → mark it `out T` → covariance                          │
│     → IFoo<Derived> can substitute for IFoo<Base>           │
│                                                             │
│  IF T appears only in PARAMETER positions (input)           │
│     → mark it `in T` → contravariance                       │
│     → IFoo<Base> can substitute for IFoo<Derived>           │
│                                                             │
│  IF T appears in BOTH positions                             │
│     → leave it unmarked → invariance                        │
│     → no substitution allowed                               │
│                                                             │
│  RULE applies only to interfaces and delegates              │
│     → never to classes or structs                           │
└─────────────────────────────────────────────────────────────┘
```

---

*If any specific section still feels unclear, revisit it slowly — the concepts build on each other. Once Part 1 (substitutability) clicks, everything else falls into place naturally.*
