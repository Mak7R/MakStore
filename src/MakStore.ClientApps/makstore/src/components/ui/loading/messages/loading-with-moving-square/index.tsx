import React from 'react';
import styles from './index.module.css'

interface LoadingWithMovingSquareProps {
  message?: string
}
export default function LoadingWithMovingSquare(props: LoadingWithMovingSquareProps) {
  return (
    <>
      <div className={styles.textWrapper}>
        <p className={styles.text}>{props.message ?? 'Loading...'}</p>
        <div className={styles.invertBox}></div>
      </div>
    </>
  );
}