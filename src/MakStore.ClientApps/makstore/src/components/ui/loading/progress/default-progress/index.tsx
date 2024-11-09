import React from 'react';
import styles from './index.module.css'

interface DefaultProgressProps {
  width?: string
}

export default function Index(props: DefaultProgressProps) {
  return (
    <>
      <div className={styles.loader} style={{width: props.width ?? '100%'}}></div>
    </>
  );
}