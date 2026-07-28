# Fargo Spa and Wellness

A responsive ASP.NET Core Blazor web application for a fictitious spa and wellness
business in Fargo, ND. Built as the CSCI 213 final project, the app lets guests
browse services and reviews, lets registered clients book and manage appointments,
and gives staff and administrators tools to run the day-to-day business.

## Roles

- **Guest** — browse the homepage, service catalog, and testimonials; register an account.
- **Client** — log in, manage their profile, book/reschedule/cancel appointments, pay online, leave reviews.
- **Service Provider** — view assigned appointments, manage availability, view/update client service notes.
- **Administrator** — manage the service catalog, master booking calendar, user roles, testimonial moderation, and reporting.

## Core domain model

| Entity | Key fields |
|---|---|
| User | UserID, FirstName, LastName, Email, PasswordHash, Role, Phone |
| Service | ServiceID, Name, Description, Duration, Price |
| Appointment | AppointmentID, ClientID, ProviderID, ServiceID, AppointmentDate, Status, ClientServiceNotes |
| Payment | PaymentID, AppointmentID, Amount, PaymentDate, PaymentStatus |
| Testimonial | TestimonialID, ClientID, Rating, ReviewText, ApprovalStatus |

A client books an appointment for a service with an assigned provider; the
appointment generates a payment, and clients may leave a testimonial afterward.

## Booking flow

Client logs in → opens the booking calendar → selects a service → the system
checks availability → client submits payment → on success the booking is saved
and a confirmation email is sent; unavailable slots or failed payments route
back to selection.

## Tech stack

- ASP.NET Core Blazor (responsive, server-rendered)
- Entity Framework Core, code-first, SQL database
- Bootstrap for layout/styling

## Running locally

```
dotnet run
```

The app listens on the URL(s) configured in `Properties/launchSettings.json`.
