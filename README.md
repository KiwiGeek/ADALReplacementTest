# ADAL Replacement sample

A simple library that replaces just exactly what I need with the ADAL library when I can't switch to MSAL

## Description

ADAL is fully deprecated, but I cxan't use MSAL because the version of ADFS we're using is too old for MSAL to work with (and no, we don't want to rush to replace it). So I extracted the functionality from ADAL that I actually care about, and implemented it.

It also does a few things that ADAL doesn't, like caching credentials, so that if a refresh token has expired, it'll refresh using those credentials. Also demonstrated is how to use ADFS to update domain credentials - although to use it in a real project you'd want to hook it up with a callback, probably. This is just how to do the functions, not how I'd expect anyone to actually do it.

## Getting Started

### Configuring

There's a bunch of CONSTS defined in `program.cs`. Adjust them as necessary for your environment. They're flagged with "Complete this" as a comment.

There's an optional compiler directive `JWT`. If defined, then the `Token` object is enhanced with additional JWT token information. This is useful if you care about the JWT. If you don't, leave it off, and you'll have less nugets included then.

The only other required Nuget is for JetBrains Annotations. This can easily be removed (along with the matching annotations) to get it down to zero dependencies.

## License

This project is licensed under the <i>The Unlicense</i> License - see the LICENSE.md file for details
