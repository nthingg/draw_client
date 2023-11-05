using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewModel.Course;
using ViewModel.Review;

namespace DrawchadViewModel.Certificate
{
    public class CertificateViewModel
    {
        public int Id { get; set; }
        public LearnerMiniViewModel Learner { get; set; }
        public CourseViewModel Course { get; set; }
        public double AverageScore { get; set; }
        public DateTime ReceiveAt { get; set; }
    }
}
