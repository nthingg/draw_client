using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewModel.Course;

namespace ViewModel.Cart
{
    public class OrderDetailViewModel
    {
        public int Id { get; set; }
        public decimal? Price { get; set; }
        public double? Rating { get; set; }
        public string? Comment { get; set; }
        public CourseViewModel Course { get; set; }
    }
}
