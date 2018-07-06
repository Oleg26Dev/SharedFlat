﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace SharedFlat
{
    public sealed class FilterTenantDbContext : ITenantDbContext
    {
        private readonly ITenantService _service;
        private string _tenantColumn = "Tenant";

        private static readonly MethodInfo _propertyMethod = typeof(EF).GetMethod(nameof(EF.Property), BindingFlags.Static | BindingFlags.Public).MakeGenericMethod(typeof(string));

        private LambdaExpression IsTenantRestriction(Type type, string tenant)
        {
            var parm = Expression.Parameter(type, "it");
            var prop = Expression.Call(_propertyMethod, parm, Expression.Constant(this._tenantColumn));
            var condition = Expression.MakeBinary(ExpressionType.Equal, prop, Expression.Constant(tenant));
            var lambda = Expression.Lambda(condition, parm);
            return lambda;
        }

        public FilterTenantDbContext(ITenantService service, string tenantColumn = "Tenant")
        {
            this._service = service;
            this._tenantColumn = tenantColumn ?? this._tenantColumn;
        }

        public void Apply(ModelBuilder modelBuilder, DbContext context)
        {
            var tenant = this._service.GetCurrentTenant();

            foreach (var entity in modelBuilder.Model.GetEntityTypes().Where(x => typeof(ITenantEntity).IsAssignableFrom(x.ClrType)))
            {
                entity.AddProperty(_tenantColumn, typeof(string));

                modelBuilder
                    .Entity(entity.ClrType)
                    .HasQueryFilter(this.IsTenantRestriction(entity.ClrType, tenant));
            }
        }
    }

}