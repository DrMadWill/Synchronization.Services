using AutoMapper;
using ConsoleTest.Events;
using ConsoleTest.Models;

namespace ConsoleTest.Mappers;

public class TestMapper: Profile
{
    public TestMapper()
    {
        CreateMap<TestInfoIntegrationEvent, DummyInfo>()
            .ReverseMap();
    }
}