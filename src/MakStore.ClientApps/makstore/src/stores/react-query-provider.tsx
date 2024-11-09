'use client'

import React from 'react';
import {QueryClient, QueryClientProvider} from "@tanstack/react-query";
import {ReactQueryDevtools} from "@tanstack/react-query-devtools";

const queryClient = new QueryClient()

interface ReactQueryProviderProps {
  children: React.ReactNode;
}

export default function ReactQueryProvider(props: ReactQueryProviderProps) {
  return (
    <>
      <QueryClientProvider client={queryClient}>
        {props.children}
        <ReactQueryDevtools initialIsOpen={false}></ReactQueryDevtools>
      </QueryClientProvider>
    </>
  );
}