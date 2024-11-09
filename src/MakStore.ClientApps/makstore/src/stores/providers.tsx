'use client'

import React from 'react';

import {SessionProvider} from "next-auth/react";

interface ProvidersProps {
  children: React.ReactNode;
}
export default function Providers(props: ProvidersProps) {
  return (
    <>
      <SessionProvider>
          {props.children}
      </SessionProvider>
    </>
  );
}