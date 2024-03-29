﻿using TestWithEF.Entities;
using TestWithEF.IRepositories;
using TestWithEF.Repository.Base;

namespace TestWithEF.Repository;

public class StandardProductRepository : Repository<StandardProduct, Guid>, IStandardProductRepository
{
    public StandardProductRepository(TestDbContext dbDbContext) : base(dbDbContext) { }
}
