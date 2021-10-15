using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Proff_MVC.Models
{
    public class Company
    {
        [Display(Name = "Name")]
        [Required]
        public string name { get; set; }
        [Display(Name = "Organisation Number")]
        [Required]
        public string organisationNumber { get; set; }

    }

    public class CompanyViewModel
    {
        public CompanyViewModel(List<Company> searchResult = null, int totalPages=1, int pageIndex=1, int pageSize=10)
        {
            this.TotalPages = totalPages;
            this.PageIndex = pageIndex;
            this.PageSize = pageSize;
            Companies = searchResult ?? new List<Company>();
        }

        ///<summary>
        /// Gets or sets Customers.
        ///</summary>
        public List<Company> Companies { get; set; }

        public int PageIndex { get; set; }

        public string SearchString { get; set; }

        public int PageSize { get; set; }

        public int TotalPages { get;  set; }

        public bool PreviousPage
        {

            get
            {
                return (PageIndex > 1);

            }
        }

        public bool NextPage
        {

            get
            {
                return (PageIndex < TotalPages);

            }
        }

        public string PreviousPageHref
        {
            get; set;
        }

        public string NextPageHref
        {
            get; set;
        }
    }

}
