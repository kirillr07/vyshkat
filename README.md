# О проекте

Консольное приложение **«ВышКат»** — система учёта парка техники и вещей. Реализовано на C# (.NET), с разделением на слои, применением принципов SOLID и DI-контейнера (`Microsoft.Extensions.DependencyInjection`). Покрыто юнит-тестами (xUnit + Moq + FluentAssertions).

---

## Запуск

```bash
cd VyshKatSolution
dotnet run --project VyshKat
```

Тесты:

```bash
dotnet test
```

---

## Структура решения

```
VyshKat/
├── Domain/            - доменная модель (не зависит от других слоёв)
│   ├── Abstractions/  - интерфейсы: IInventoryItem, IEnergyConsumer, INoviceFriendly, IFleetRepository
│   ├── Vehicles/      - Vehicle, Scooter, Bicycle, ElectricBicycle
│   ├── Gear/          - GearItem, Helmet, DockingStation, ChargingCabinet
│   └── Services/      - IServiceCenter, ServiceCenter
├── Application/       - сценарии использования: FleetService, FleetReportService
├── Infrastructure/    - FleetRepository (in-memory реализация IFleetRepository)
├── Presentation/      - ConsoleAppMenu (консольный интерфейс)
└── Program.cs         - точка входа, настройка DI

VyshKat.Tests/
├── Domain/            - ServiceCenterTests
└── Application/       - FleetServiceTests, FleetReportServiceTests
```

**Почему так:**
- `Domain` не знает ни о `Application`, ни об `Infrastructure` - зависимости направлены внутрь.
- `Application` зависит только от абстракций `Domain`.
- `Infrastructure` реализует интерфейс репозитория, объявленный в `Domain`.
- `Presentation` использует сервисы `Application`, не лезет в домен напрямую.

---

## SOLID — что и где

### SRP (Single Responsibility)
- `FleetService` - приём техники (проверка через техосмотр + добавление в репозиторий).
- `FleetReportService` - только отчёты (энергия, новички, подсчёт по типам).
- `ServiceCenter` - только решение о приёме.
- `FleetRepository` - только хранение.
- `ConsoleAppMenu` - только ввод/вывод.

### OCP (Open/Closed)
- Новый вид транспорта = новый класс-наследник `Vehicle` (например, `Monowheel`). Ни `FleetService`, ни `ServiceCenter`, ни `FleetRepository` менять не нужно.
- Отчёты работают через `OfType<IEnergyConsumer>()` / `OfType<INoviceFriendly>()`, а не через `switch` по конкретным типам.

### LSP (Liskov Substitution)
- `Scooter`, `Bicycle`, `ElectricBicycle` подставляются вместо `Vehicle` без изменения поведения.
- `Helmet`, `DockingStation`, `ChargingCabinet` — вместо `GearItem`.

### ISP (Interface Segregation)
- Интерфейсы разделены: `IInventoryItem` (номер + имя), `IEnergyConsumer` (расход), `INoviceFriendly` (рейтинг).
- `Helmet` не обязан реализовывать `IEnergyConsumer`. `Scooter` не реализует `IEnergyConsumer`.

### DIP (Dependency Inversion)
- `FleetService` зависит от `IFleetRepository` и `IServiceCenter`, а не от конкретных `FleetRepository` / `ServiceCenter`.
- `FleetReportService` зависит от `IFleetRepository`.
- Всё связывается в `Program.cs` через DI.

**Наиболее явно применён DIP:** в `FleetService` и `FleetReportService` — обе зависимости подставляются через конструктор, конкретные реализации выбираются в `Program.cs`. Именно это позволяет в тестах подменить `IServiceCenter` моком.

---

## Сознательно ограниченный принцип

**SRP в `ConsoleAppMenu`.** Класс делает и меню, и парсинг ввода, и вывод отчётов, и сборку объектов. Это несколько ответственностей, но здесь это приемлемо, потому что:
- это единственный класс представления, он не переиспользуется;
- выделение отдельных `InputReader` / `OutputWriter` дало бы больше кода, чем пользы;
- логика решений (техосмотр, приём, отчёты) вынесена в `Application` и `Domain`, то есть тестируется без консоли.

---

## Что изменяется при расширении

### Новый вид транспорта (например, `Monowheel`)
Добавляется класс `Monowheel : Vehicle, INoviceFriendly` (и, при необходимости, `IEnergyConsumer`). Изменения:
- в `ConsoleAppMenu.AcceptDevice()` - одна новая ветка `switch` и метод-билдер.

`FleetService`, `FleetReportService`, `ServiceCenter`, `FleetRepository` - **не меняются**. Отчёты подхватят новый тип автоматически (`OfType<IEnergyConsumer>`, `GroupBy(GetType().Name)`).

---

## Тесты

### Тест, фиксирующий решение техосмотра
`ServiceCenterTests.Approve_ReturnsFalse_ForInvalidNoviceRating` (Theory: 0, 11, -1) и `Approve_ReturnsTrue_ForValidNoviceRating` (Theory: 1, 6, 10) фиксируют границы допустимого рейтинга.

Также `Approve_ReturnsFalse_ForChargingCabinet_WithNegativeEnergy` - правило «расход не может быть отрицательным».

### Тест, фиксирующий правило «для новичков»
`FleetReportServiceTests.GetNoviceFriendlyItems_ReturnsOnlyItemsWithRatingAtLeastSix`:
- создаётся `Scooter` с рейтингом 8, `Scooter` с рейтингом 4, `Bicycle` с рейтингом 6, `Helmet`;
- проверяется, что в выборке ровно 2 элемента и все с `NoviceRating >= 6`.

Само правило `IsSuitableForNovice => NoviceRating >= 6` объявлено в `INoviceFriendly` как default-реализация - это тоже часть фиксации.

### Как подменяется зависимость
В `FleetServiceTests`:
```csharp
private readonly Mock<IFleetRepository> _repository = new();
private readonly Mock<IServiceCenter> _serviceCenter = new();
private readonly FleetService _sut;

public FleetServiceTests()
{
    _sut = new FleetService(_repository.Object, _serviceCenter.Object);
}
```

Тест `TryAccept_ReturnsFalse_WhenServiceCenterRejects`:
```csharp
_serviceCenter.Setup(s => s.Approve(scooter)).Returns(false);
var result = _sut.TryAccept(scooter);
result.Should().BeFalse();
_repository.Verify(r => r.Add(It.IsAny<IInventoryItem>()), Times.Never);
```
Здесь `IServiceCenter` подменён моком, который всегда отклоняет и проверяется, что репозиторий не вызывается. Это прямая проверка DIP: `FleetService` не знает, что за техосмотр ему подсунули.

Аналогично в `FleetReportServiceTests` подменяется `IFleetRepository` - отчёты тестируются без реального хранилища.

---

## Роль ИИ

**Что сделал ИИ:**
- сгенерировал `ConsoleAppMenu.cs` (меню, ввод, вывод, билдеры объектов);
- сгенерировал тесты (`ServiceCenterTests`, `FleetServiceTests`, `FleetReportServiceTests`).

**Что сделано самостоятельно:**
- вся доменная модель (`Domain/Abstractions`, `Domain/Vehicles`, `Domain/Gear`, `Domain/Services`);
- `FleetService`, `FleetReportService`;
- `FleetRepository`;
- `Program.cs` (настройка DI);
- архитектура решения и распределение по слоям.
