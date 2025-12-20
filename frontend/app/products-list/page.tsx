import { fetchAPI } from '@/lib/serverApi';
import type { Product, Category, PaginatedResponse } from '@/types';
import ProductsListClient from './ProductsListClient';

const PAGE_SIZE = 5;

interface ProductsListPageProps {
  searchParams: {
    category?: string;
    page?: string;
  };
}

export default async function ProductsListPage({ searchParams }: ProductsListPageProps) {
  // Parse search params
  const categoryId = searchParams.category ? parseInt(searchParams.category, 10) : null;
  const pageNumber = searchParams.page ? parseInt(searchParams.page, 10) : 1;
  const validPageNumber = pageNumber > 0 ? pageNumber : 1;
  const validCategoryId = categoryId && !isNaN(categoryId) ? categoryId : null;

  // Fetch data on server
  let productsResponse: PaginatedResponse<Product>;
  let categoriesResponse: Category[];

  try {
    [productsResponse, categoriesResponse] = await Promise.all([
      fetchAPI<PaginatedResponse<Product>>(
        `/Products/paginated?pageNumber=${validPageNumber}&pageSize=${PAGE_SIZE}${validCategoryId ? `&categoryId=${validCategoryId}` : ''}`
      ),
      fetchAPI<Category[]>('/Categories'),
    ]);
  } catch (error) {
    console.error('Error loading products:', error);
    // Return empty state on error
    productsResponse = {
      data: [],
      pageNumber: 1,
      pageSize: PAGE_SIZE,
      totalCount: 0,
      totalPages: 0,
      hasPreviousPage: false,
      hasNextPage: false,
    };
    categoriesResponse = [];
  }

  return (
    <ProductsListClient
      initialProducts={productsResponse.data}
      initialCategories={categoriesResponse}
      initialPageNumber={validPageNumber}
      initialSelectedCategory={validCategoryId}
      initialTotalCount={productsResponse.totalCount}
      initialTotalPages={productsResponse.totalPages}
      pageSize={PAGE_SIZE}
    />
  );
}

