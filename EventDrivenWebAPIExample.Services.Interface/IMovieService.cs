using System.Collections.Generic;
using System.Threading.Tasks;
using EventDrivenWebAPIExample.Domain;
using EventDrivenWebAPIExample.Domain.Mongo;

namespace EventDrivenWebAPIExample.Services.Interface
{
    public interface IMovieService
    {
        Task<Movie> FindMovieByTitleAsync(string title);
        List<KafkaReturnValue> SendMovie(Movie movie);
    }
}
