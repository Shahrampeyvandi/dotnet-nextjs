# OTP Registration with Kavehnegar SMS - Event-Driven Implementation

This document describes the OTP (One-Time Password) registration system implemented using an event-driven architecture with Kavehnegar SMS provider.

## Architecture Overview

The implementation follows an **event-driven architecture** using MediatR:

1. **OTP Request Flow:**
   - User requests OTP → `OtpService.GenerateOtpAsync()` 
   - OTP code is generated and stored
   - `OtpRequestedEvent` is published
   - `OtpRequestedEventHandler` receives the event
   - SMS is sent via Kavehnegar (asynchronously, non-blocking)

2. **OTP Verification Flow:**
   - User submits OTP code
   - `OtpService.VerifyOtpAsync()` validates the code
   - If valid, `OtpVerifiedEvent` is published
   - `OtpVerifiedEventHandler` logs the verification

3. **Registration Flow:**
   - User provides registration data + OTP code
   - OTP is verified first
   - If valid, user account is created

## Configuration

### 1. Kavehnegar API Setup

Add your Kavehnegar API credentials to `appsettings.json`:

```json
{
  "Sms": {
    "Kavehnegar": {
      "ApiKey": "YOUR_KAVEHNEGAR_API_KEY",
      "SenderNumber": "10001001010",
      "ApiUrl": "https://api.kavenegar.com/v1/{0}/sms/send.json"
    }
  }
}
```

**To get your API key:**
1. Register at [Kavehnegar](https://panel.kavenegar.com/client/membership/register)
2. Get your API key from the panel
3. Set your sender number (must be verified in Kavehnegar panel)

### 2. Database Migration

Create and apply a migration for the `OtpCode` entity:

```bash
dotnet ef migrations add AddOtpCodeEntity --project testttt.Infrastructure --startup-project testttt.Presentation
dotnet ef database update --project testttt.Infrastructure --startup-project testttt.Presentation
```

## API Endpoints

### 1. Request OTP

**Endpoint:** `POST /api/Auth/request-otp`

**Request Body:**
```json
{
  "phoneNumber": "09123456789",
  "purpose": "Registration"
}
```

**Response:**
```json
{
  "success": true,
  "message": "OTP code has been sent to your phone number",
  "expirationMinutes": 5
}
```

**Note:** The SMS is sent asynchronously via event handler. The API returns immediately without waiting for SMS delivery.

### 2. Verify OTP

**Endpoint:** `POST /api/Auth/verify-otp`

**Request Body:**
```json
{
  "phoneNumber": "09123456789",
  "code": "123456",
  "purpose": "Registration"
}
```

**Response:**
```json
{
  "success": true,
  "message": "OTP verified successfully"
}
```

### 3. Register with OTP

**Endpoint:** `POST /api/Auth/register-with-otp`

**Request Body:**
```json
{
  "username": "john_doe",
  "email": "john@example.com",
  "password": "SecurePass123",
  "firstName": "John",
  "lastName": "Doe",
  "phoneNumber": "09123456789",
  "otpCode": "123456"
}
```

**Response:**
```json
{
  "id": "user-id",
  "username": "john_doe",
  "email": "john@example.com",
  "firstName": "John",
  "lastName": "Doe",
  "phone": "09123456789",
  "roles": [],
  "createdAt": "2024-01-01T00:00:00Z"
}
```

## Event-Driven Components

### Events

1. **OtpRequestedEvent** - Published when an OTP is generated
   - Triggers SMS sending via event handler
   - Non-blocking, asynchronous

2. **OtpVerifiedEvent** - Published when OTP is successfully verified
   - Can be used for logging, analytics, etc.

### Event Handlers

1. **OtpRequestedEventHandler** - Handles `OtpRequestedEvent`
   - Sends SMS via Kavehnegar
   - Logs success/failure

2. **OtpVerifiedEventHandler** - Handles `OtpVerifiedEvent`
   - Logs verification events
   - Can be extended for analytics

## Key Features

- ✅ **Event-Driven Architecture** - Loose coupling, easy to extend
- ✅ **Asynchronous SMS Sending** - Non-blocking API responses
- ✅ **OTP Expiration** - Codes expire after 5 minutes (configurable)
- ✅ **OTP Reuse Prevention** - Codes can only be used once
- ✅ **Phone Number Normalization** - Automatically handles Iranian phone numbers
- ✅ **Comprehensive Logging** - All OTP operations are logged

## Phone Number Format

The system automatically normalizes phone numbers:
- Accepts formats: `09123456789`, `00989123456789`, `+989123456789`
- Normalizes to: `989123456789` (Iran country code + number)

## Security Considerations

1. **OTP Expiration:** OTP codes expire after 5 minutes
2. **Single Use:** Each OTP can only be used once
3. **Rate Limiting:** Consider adding rate limiting to prevent abuse
4. **Phone Verification:** Phone numbers are marked as confirmed after OTP verification

## Testing

For testing without actual SMS sending, you can:
1. Mock the `ISmsService` in your tests
2. Check the database for generated OTP codes
3. Use the OTP code from database for verification

## Extending the System

To add new OTP purposes (e.g., password reset, login):

1. Use different `purpose` values when calling `GenerateOtpAsync()`
2. The same event handlers will work for all purposes
3. You can add purpose-specific handlers if needed

## Troubleshooting

### SMS Not Sending
- Check Kavehnegar API key and sender number in configuration
- Verify phone number format
- Check application logs for errors
- Ensure Kavehnegar account has sufficient credit

### OTP Verification Failing
- Check if OTP has expired (5 minutes)
- Verify OTP hasn't been used already
- Check phone number matches exactly (including country code)

