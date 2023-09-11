using Silerium.Models;

namespace Silerium.Services.PaginationServices
{
    public class ModelPaginationService
    {
        public static void CountPages(PaginationModel paginationModel, int itemsCount)
        {
            if (paginationModel.CurrentPage > paginationModel.LastPageIndex && paginationModel.CurrentPage * itemsCount < itemsCount)
            {
                paginationModel.FirstPageIndex = (paginationModel.LastPageIndex + 1) * (paginationModel.PageMultiplier - 1);
                paginationModel.LastPageIndex++;
                for (int i = PaginationModel.MaxItemsAtPage; i < paginationModel.CurrentPage * PaginationModel.MaxItemsAtPage; i += PaginationModel.MaxItemsAtPage)
                {
                    paginationModel.LastPageIndex++;
                    paginationModel.PageMultiplier++;
                }
            }
            else
                paginationModel.CurrentPage = paginationModel.LastPageIndex;
            for (int i = 0; i < (int)MathF.Floor(itemsCount / PaginationModel.MaxItemsAtPage); i++)
            {
                if (itemsCount * paginationModel.LastPageIndex >= PaginationModel.MaxItemsAtPage * paginationModel.LastPageIndex)
                {
                    paginationModel.LastPageIndex++;
                }
            }
        }
        public static bool ItemOnPage(PaginationModel paginationModel, int itemIndex)
        {
            return itemIndex >= (paginationModel.CurrentPage - 1) * PaginationModel.MaxItemsAtPage
                && itemIndex < paginationModel.CurrentPage * PaginationModel.MaxItemsAtPage;
        }
    }
}
