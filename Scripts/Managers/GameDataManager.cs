using Godot;
using System;
using System.Collections.Generic;
using System.Text.Json;

public partial class PortalModel
{
  public string Name { get; set; }
  public string Category { get; set; }
  public ProductModel[] Products { get; set; }
  public string Prefix { get; set; }

  public override string ToString()
  {
    return $"Name: {Name}, Category: {Category}, Products: {string.Join(", ", (IEnumerable<ProductModel>)Products)}, Prefix: {Prefix}";
  }
}

public partial class CategoryModel
{
  public string Name { get; set; }
  public string Icon { get; set; }

  public override string ToString()
  {
    return $"Name: {Name}, Icon: {Icon}";
  }
}

public partial class ProductModel
{
  public string Name { get; set; }
  public string Prefix { get; set; }
  public string Icon { get; set; }
  public int Price { get; set; }

  public override string ToString()
  {
    return $"Name: {Name}, Prefix: {Prefix}, Icon: {Icon}, Price: {Price}";
  }
}

public enum PortalType
{
  StellarEmporium,
  GalacticForge,
  DimensionalRiftCafe,
  TechnomancerSanctuary,
  AstralOperaHouse,
  BlackHoleBazaar,
  TimekeepersGuild,
  CelestialFarm,
  QuantumSpa,
  NebulaLibrary,
  VoidCasino,
  HyperNovaResort,
  InfiniteEchoMuse,
  EventHorizonCafe,
  StarshipWorkshop,
  InterdimensionalEmbassy,
  CelestialDiner
}

public partial class GameDataManager : Node
{
  public static GameDataManager Instance;
  private Dictionary<PortalType, PortalModel> portalModels = new();
  private Dictionary<string, CategoryModel> categoryModels = new();
  private Random random = new Random();

  public GameDataManager()
  {
    Instance = this;
  }

  public override void _Ready()
  {
    LoadCategoryModels();
    LoadPortalModels();
  }

  #region Category Models
  private void LoadCategoryModels()
  {
    string json = System.IO.File.ReadAllText(ProjectSettings.GlobalizePath($"{Refs.In.PortalsDataPath}Categories.json"));

    try
    {
      var models = JsonSerializer.Deserialize<Dictionary<string, CategoryModel>>(json);

      if (models == null)
      {
        GD.PrintErr("Error parsing JSON");
        return;
      }

      foreach (var model in models)
      {
        categoryModels[model.Key] = model.Value;
      }
    }
    catch (Exception e)
    {
      GD.PrintErr(e.Message);
    }
  }

  public CategoryModel GetCategory(string name)
  {
    try
    {
      return categoryModels[name];
    }
    catch (KeyNotFoundException)
    {
      GD.PrintErr($"Category {name} not found");
      return null;
    }
  }

  #endregion

  #region Portal Models
  private void LoadPortalModels()
  {
    string json = System.IO.File.ReadAllText(ProjectSettings.GlobalizePath($"{Refs.In.PortalsDataPath}Portals.json"));

    try
    {
      var models = JsonSerializer.Deserialize<Dictionary<string, PortalModel>>(json);

      if (models == null)
      {
        GD.PrintErr("Error parsing JSON");
        return;
      }

      foreach (var model in models)
      {
        if (Enum.TryParse(model.Key, out PortalType type))
        {
          portalModels[type] = model.Value;
        }
      }
    }
    catch (Exception e)
    {
      GD.PrintErr(e.Message);
    }
  }

  public PortalModel GetPortal(PortalType type)
  {
    return portalModels[type];
  }

  public PortalModel GetRandomPortal()
  {
    var values = Enum.GetValues(typeof(PortalType));
    var randomType = (PortalType)values.GetValue(random.Next(values.Length));

    return portalModels[randomType];
  }

  #endregion

  public ProductModel GetRandomProduct(PortalType type)
  {
    var products = GetPortal(type).Products;
    return products[random.Next(products.Length)];
  }
}
