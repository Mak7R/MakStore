import React, {InputHTMLAttributes} from 'react';
import styles from './index.module.css'
interface DefaultCheckboxProps extends InputHTMLAttributes<HTMLInputElement> {

}

export default function DefaultCheckbox(props: DefaultCheckboxProps) {
  return (
    <label className="relative inline-flex items-center cursor-pointer">
      <input type="checkbox" value="" className="sr-only peer" {...props} />
      <div className="peer ring-0 bg-rose-400  rounded-full outline-none duration-300 after:duration-500 w-12 h-12  shadow-md peer-checked:bg-emerald-500  peer-focus:outline-none  after:content-['✖️'] after:rounded-full after:absolute after:outline-none after:h-10 after:w-10 after:bg-gray-50 after:top-1 after:left-1 after:flex after:justify-center after:items-center  peer-hover:after:scale-75 peer-checked:after:content-['✔️'] after:-rotate-180 peer-checked:after:rotate-0"></div>
    </label>
  );
}