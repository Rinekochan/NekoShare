# NekoShare Platform

- This is a .NET + Angular project that represents a social media platform. The published code can be found in **publish** branch

## How to use?
- Link: https://neko-share-eph2ffhdbqcdcxcw.eastasia-01.azurewebsites.net/
- The application is pre-seeded for demonstration, you can use this to get through:
    - demone - Dem0@pps (Moderators)
    - demotwo - Dem0@pps

## Features
- This application allows users to find another users in the platform. They can see details, like them, and even message them.
    - There are pagination features where users can filter, search for users.
- The real-time functionality is integrated to message feature using SignalR. Users can also track if their friends are online or not in real-time.
    -  The user is also being notified if they received a new message or somebody is online.
- The application allows for photo storage and display. However each photo requires an admin approval before users can make it as their avatar.
- The application follows role-based access control model, where there are three roles: Users, Moderators, Admins.
    -  Moderators can approve or reject new photo uploads.
    -  Admin can remove Users from the platform.
- Users is authenticated and authorized via JWT token, which will be added to the request header before sending back to the server for authorized operations (It also follows RBAC policy).
