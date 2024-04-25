using GaEpd.AppLibrary.Pagination;
using MyApp.AppServices.Complaints.QueryDto;

namespace MyApp.WebApp.Models;

public record SearchResultsDisplay(
    IBasicSearchDisplay Spec,
    IPaginatedResult<ComplaintSearchResultDto> SearchResults,
    PaginationNavModel Pagination,
    bool IsPublic)
{
    public string SortByName => Spec.Sort.ToString();
}
