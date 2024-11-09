import React from 'react';

interface MainComponentProps {
  children?: React.ReactNode;
}

export default function MainComponent(props: MainComponentProps) {
  return (
    <main>
      {props.children}
    </main>
  );
}