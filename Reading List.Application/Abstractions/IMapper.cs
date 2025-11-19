using Reading_List.Domain.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reading_List.Application.Abstractions
{
    public interface IMapper<TEntity, TDto>
        where TEntity : class, IEntity
        where TDto : class, IEntity
    {
        TDto toDto(TEntity entity);

        TEntity toEntity(TDto dto);
    }
}
