using ControlWatch.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControlWatch.Commons.Services
{
    public interface IMovieService
    {
        List<Movie> GetMovies();
    }
}
