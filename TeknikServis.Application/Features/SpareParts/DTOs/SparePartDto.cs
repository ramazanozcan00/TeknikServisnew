using System;

namespace TeknikServis.Application.Features.SpareParts.DTOs
{
    public record SparePartDto(
        Guid Id,
        string Name,
        string Code,
        decimal PurchasePrice,
        decimal SalePrice,
        int StockQuantity,
        int CriticalStockLevel,
        string Unit,
        DateTime CreatedAt);
}