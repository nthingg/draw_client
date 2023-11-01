using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModel.Review
{
    public class ReviewViewModel
    {
        public LearnerMiniViewModel Learner { get; set; }
        public double Rating { get; set; }
        public string Comment { get; set; }
    }
}
