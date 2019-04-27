/// <summary>
/// Interface for each item which can be sold/bought in the trader hut
/// </summary>
public interface ITradeable
{
    /// <summary>
    /// Indicates whether item can be traded
    /// </summary>
    bool Tradeable { get; set; }

    /// <summary>
    /// Coins to receive when selling
    /// </summary>
    int SellPrice { get; set; }

    /// <summary>
    /// Coins to give away when buying
    /// </summary>
    int BuyPrice { get; set; }
}