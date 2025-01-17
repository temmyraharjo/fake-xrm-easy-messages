﻿using Crm;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using System;
using System.Linq;
using Xunit;

namespace FakeXrmEasy.Messages.Tests.FakeMessageExecutors.SetStateRequestTests
{
    public class SetStateRequestTests: FakeXrmEasyTestsBase
    {
        [Fact]
        public void When_set_state_request_is_called_an_entity_is_updated()
        {
            var c = new Contact()
            {
                Id = Guid.NewGuid()
            };
            _context.Initialize(new[] { c });

            var request = new SetStateRequest
            {
                EntityMoniker = c.ToEntityReference(),
                State = new OptionSetValue(69),
                Status = new OptionSetValue(6969),
            };

            var response = _service.Execute(request);

            //Retrieve record after update
            var contact = (from con in _context.CreateQuery<Contact>()
                           where con.Id == c.Id
                           select con).FirstOrDefault();

            Assert.Equal(69, (int)contact.StateCode.Value);
            Assert.Equal(6969, contact.StatusCode.Value);
        }

        [Fact]
        public void Should_set_a_statecode_by_default_when_an_entity_record_is_added_to_the_context()
        {
            var c = new Contact()
            {
                Id = Guid.NewGuid()
            };
            _context.Initialize(new[] { c });

            //Retrieve record after update
            var contact = (from con in _context.CreateQuery<Contact>()
                           where con.Id == c.Id
                           select con).FirstOrDefault();

            Assert.Equal(0, (int)contact.StateCode.Value); //Active
        }

        [Fact]
        public void Should_not_override_a_statecode_already_initialized()
        {
            var c = new Contact()
            {
                Id = Guid.NewGuid(),
            };

            c["statecode"] = new OptionSetValue(69); //As the StateCode is read only in the early bound entity, this is the only way of updating it

            _context.Initialize(new[] { c });

            //Retrieve record after update
            var contact = (from con in _context.CreateQuery<Contact>()
                           where con.Id == c.Id
                           select con).FirstOrDefault();

            Assert.Equal(69, (int)contact.StateCode.Value); //Set
        }
    }
}