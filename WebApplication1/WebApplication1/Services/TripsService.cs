﻿using System.Data;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Context;
using WebApplication1.DTOs;
using WebApplication1.Models;


namespace WebApplication1;

public class TripsService : ITripsService
{
    private Apdb9Context _context;

    public TripsService(Apdb9Context context)
    {
        _context = context;
    }
    public async Task<PagesDto> GetTrips(string? query, int? page, int? pageSize)
    {
        //domyslne ustawienie wartosci na wypadek gdyby parametry nie zostaly przekazane
        var currentPage = page ?? 1;
        var currentPageSize = pageSize ?? 10;
        //inicjalizujemy zapytanie jako Queryable
        var tripsQuery =  _context.
            Trips.
            Include(t=>t.IdCountries).//włączamy pobieranie danych krajów połączonych z wycieczka
            Include(t=>t.ClientTrips).//włączamy pobieranie danych klientow połączonych z wycieczka
            ThenInclude(t=>t.IdClientNavigation).AsQueryable();//pobieramy dane klientów 

        //jezeli zostal przekazany parametr query to wtedy filtrijemy wyniki na jego podstawie
        if (!string.IsNullOrEmpty(query))
        {
            tripsQuery =  tripsQuery.Where(t => t.Name.Contains(query));
        }
        
        //pobieramy liczbę wyników 
        var tripsNumber = await tripsQuery.CountAsync();
        
        
        //stronnicujemy wyniki
        var tripList = await tripsQuery.
                                Skip((currentPage - 1) * currentPageSize).//pomijamy elementy na poprzednich stronach
                                        Take(currentPageSize).//pobieramy okreslona ilosc  elementow dla biezacegj strony
                                            ToListAsync();
        var result = new PagesDto()
        {
            pageNum = currentPage,
            pageSize = currentPageSize,
            allPages = tripsNumber / pageSize ?? 10,
            trips = tripList.Select(t => new TripDto()
            {
                Name = t.Name,
                Description = t.Description,
                DateFrom = t.DateFrom,
                DateTo = t.DateTo,
                MaxPeople = t.MaxPeople,
                Countries = t.IdCountries.Select(c => new CountryDto() { Name = c.Name }).ToList(),
                Clients = t.ClientTrips.Select(c => new ClientDto()
                {
                    FirstName = c.IdClientNavigation.FirstName,
                    LastName = c.IdClientNavigation.LastName
                }).ToList(),

            }).ToList()
        };
        return result;
    }

    public async Task<string> DeleteClient(int id)
    {
        

        var isClientExists = await _context.Clients.FindAsync(id);

        if (isClientExists == null)
        {
            throw new DataException("Client with this Id does not exists");
        }
        
        var clientCount = await _context.ClientTrips.
            Where(c => c.IdClient == id).
                CountAsync();
        if (clientCount!=0)
        {
            throw new DataException("Client can not be deleted, because he has trips");
        }

        _context.Clients.Remove(isClientExists);
        return "Client removed sucesfully";

    }

    public async Task<string> AddClientToTrip(int tripId, NewClientDto newClient)
    {
        
        var clientExists = await _context.Clients.AnyAsync(c => c.Pesel == newClient.Pesel);
        if (clientExists)
        {
            throw new DataException("Client with this PESEL already exists");
        }

        
        var clientAtTripExists = await _context.ClientTrips
            .Include(ct => ct.IdClientNavigation) 
            .AnyAsync(ct => ct.IdTrip == tripId && ct.IdClientNavigation.Pesel == newClient.Pesel);
        if (clientAtTripExists)
        {
            throw new DataException("Client with this PESEL already has a trip with this id");
        }

        
        var trip = await _context.Trips.FindAsync(tripId);
        if (trip == null)
        {
            throw new DataException("Trip with this Id does not exists");
        }

       
        if (DateTime.Now > trip.DateFrom)
        {
            throw new DataException("The trip has already started");
        }

        
        var clientToAdd = new Client()
        {
            FirstName = newClient.FirstName,
            LastName = newClient.LastName,
            Email = newClient.Email,
            Telephone = newClient.Telephone,
            Pesel = newClient.Pesel
        };
        _context.Clients.Add(clientToAdd);
        await _context.SaveChangesAsync();

        
        var clientTripToAdd = new ClientTrip()
        {
            IdClient = clientToAdd.IdClient,
            IdTrip = tripId,
            RegisteredAt = DateTime.Now,
            PaymentDate = newClient.PaymentDate
        };
        _context.ClientTrips.Add(clientTripToAdd);
        await _context.SaveChangesAsync();

        return "Client successfully added to a trip";
    }
}