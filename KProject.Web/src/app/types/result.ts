interface ValidationError {
  code: string;
  description: string;
}

type Result<T> =
  | {success: true, data?: T}
  | {success: false, errors: ValidationError[]}
