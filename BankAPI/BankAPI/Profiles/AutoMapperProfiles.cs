using AutoMapper;
using BankAPI.Models;

namespace BankAPI.Profiles
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            //dto classes
            CreateMap<RegisterNewAccountModel, Account>();
            CreateMap<UpdateAccountModel, Account>();
            CreateMap<Account, GetAccountModel>();
            CreateMap<TransactionRequestDto, Transaction>();
            CreateMap<Transaction, GetTransactionModel>();
            CreateMap<GetTransactionModel, Response>().ReverseMap();
            CreateMap<Response, Transaction>().ReverseMap();
        }
    }
}
