// Category Types
export interface Category {
  id: number;
  name: string;
  description?: string;
  createdAt: string;
  updatedAt?: string;
}

export interface CreateCategoryDto {
  name: string;
  description?: string;
}

export interface UpdateCategoryDto {
  name: string;
  description?: string;
}

// Product Types
export interface Product {
  id: number;
  name: string;
  description?: string;
  price: number;
  discountPercentage?: number;
  discountStartDate?: string;
  discountEndDate?: string;
  finalPrice?: number;
  hasActiveDiscount?: boolean;
  stockQuantity: number;
  imageUrl?: string;
  isActive: boolean;
  createdAt: string;
  updatedAt?: string;
  categoryId: number;
  categoryName?: string;
}

export interface CreateProductDto {
  name: string;
  description?: string;
  price: number;
  discountPercentage?: number;
  discountStartDate?: string;
  discountEndDate?: string;
  stockQuantity: number;
  imageUrl?: string;
  isActive: boolean;
  categoryId: number;
}

export interface UpdateProductDto {
  name: string;
  description?: string;
  price: number;
  discountPercentage?: number;
  discountStartDate?: string;
  discountEndDate?: string;
  stockQuantity: number;
  imageUrl?: string;
  isActive: boolean;
  categoryId: number;
}

// Customer Types
export interface Customer {
  id: number;
  firstName: string;
  lastName: string;
  email: string;
  phone?: string;
  address?: string;
  city?: string;
  postalCode?: string;
  createdAt: string;
  updatedAt?: string;
}

export interface CreateCustomerDto {
  firstName: string;
  lastName: string;
  email: string;
  phone?: string;
  address?: string;
  city?: string;
  postalCode?: string;
}

export interface UpdateCustomerDto {
  firstName: string;
  lastName: string;
  email: string;
  phone?: string;
  address?: string;
  city?: string;
  postalCode?: string;
}

// Order Types
export interface OrderItem {
  id: number;
  quantity: number;
  unitPrice: number;
  totalPrice: number;
  productId: number;
  productName?: string;
}

export interface Order {
  id: number;
  orderNumber: string;
  orderDate: string;
  totalAmount: number;
  status: string;
  shippingAddress?: string;
  createdAt: string;
  updatedAt?: string;
  customerId?: number;
  userId?: string;
  customerName?: string;
  customerEmail?: string;
  customerPhone?: string;
  itemsCount?: number;
  orderItems?: OrderItem[]; // Optional for backward compatibility
}

// Flat DTO for paginated order lists (no nested orderItems)
export interface OrderListDto {
  id: number;
  orderNumber: string;
  orderDate: string;
  totalAmount: number;
  status: string;
  shippingAddress?: string;
  createdAt: string;
  updatedAt?: string;
  customerId?: number;
  userId?: string;
  customerName?: string;
  customerEmail?: string;
  customerPhone?: string;
  itemsCount: number;
}

export interface CreateOrderItemDto {
  productId: number;
  quantity: number;
}

export interface CreateOrderDto {
  customerId: number;
  shippingAddress?: string;
  orderItems: CreateOrderItemDto[];
}

export interface UpdateOrderDto {
  status: string;
  shippingAddress?: string;
}

export interface PaginatedResponse<T> {
  data: T[];
  pageNumber: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
  hasPreviousPage: boolean;
  hasNextPage: boolean;
}

// User Types
export interface User {
  id: string; // Changed to string for Identity
  username: string;
  email: string;
  firstName: string;
  lastName: string;
  phone?: string;
  address?: string;
  city?: string;
  postalCode?: string;
  createdAt: string;
  roles?: string[]; // User roles from Identity
}

export interface RegisterDto {
  username: string;
  email: string;
  password: string;
  firstName: string;
  lastName: string;
}

export interface LoginDto {
  username: string;
  password: string;
}

export interface UpdateUserDto {
  firstName: string;
  lastName: string;
  email: string;
  phone?: string;
  address?: string;
  city?: string;
  postalCode?: string;
}

// Cart Types
export interface CartItem {
  productId: number;
  productName: string;
  price: number;
  quantity: number;
  totalPrice: number;
  imageUrl?: string;
}

export interface Cart {
  items: CartItem[];
  totalAmount: number;
  totalItems: number;
}

export interface AddToCartDto {
  productId: number;
  quantity: number;
}

